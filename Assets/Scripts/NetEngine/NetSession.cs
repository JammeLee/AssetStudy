using System;
using System.Net.Sockets;
using UnityEngine.Assertions;

namespace MFrame.NetEngine
{
    public class NetSession : INetSession
    {
        #region Public
        
        public bool Connecting { get; private set; }
        public bool Connected
        {
            get { return m_TcpClient != null && m_TcpClient.Connected; }
        }
        public Exception LastError { get; private set; }
        public int Latency { get; set; }
        public Action<Exception> OnException { get; set; }
        public INetMsg Msg { get; set; }
        
        public delegate INetMsg CreateNetMsgDelegate(byte[] buffer, int index, int length);

        #endregion

        #region Private

        private CreateNetMsgDelegate m_MsgCreator;

        private INetSessionEvent m_Event;

        private AsyncCallback m_WriteCallback;

        private TcpClient m_TcpClient;

        private const int BUFF_LEN = 1024 * 10;

        private byte[] m_ReadBuffer = new byte[BUFF_LEN];

        private int m_StartPos = 0; //读取游标：buffer当前已读取到的位置的下标

        private int m_EndPos = 0; //存储游标：buffer当前已存储的位置的下标 buffer的size，不是length

        #endregion

        private void onConnected(IAsyncResult ar)
        {
            var session = ar.AsyncState as NetSession;
            Connecting = false;
            try
            {
                m_TcpClient.EndConnect(ar);
                if (m_TcpClient.Connected)
                {
                    if (m_Event != null)
                    {
                        m_Event.OnConnected(this);
                        
                    }
                }
            }
            catch (Exception e)
            {
                onException(e);
            }
        }

        private void onReadStream(IAsyncResult ar)
        {
            try
            {
                var stream = m_TcpClient.GetStream();
                //更新buffer的size
                m_EndPos += stream.EndRead(ar);

                //开始读取msg内容
                while (m_StartPos < m_EndPos)
                {
                    var len = m_EndPos - m_StartPos;
                    if (Msg == null)
                    {
                        Msg = m_MsgCreator.Invoke(m_ReadBuffer, m_StartPos, len);
                    }

                    if (Msg != null)
                    {
                        int numRead = Msg.ReadBuffer(m_ReadBuffer, m_StartPos, len);
                        if (numRead <= 0)
                        {
                            throw new Exception("read data error");
                        }
                        // 更新读取游标
                        m_StartPos += numRead;
                        if (Msg.IsComplete())
                        {
                            Msg.Deserialize();
                            if (m_Event != null)
                            {
                                m_Event.OnMessageRead(this, Msg);
                            }
                            Msg = null;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            
                BeginRead();
            }
            catch (Exception e)
            {
                onException(e);
            }
        }

        private void onWriteStream(IAsyncResult ar)
        {
            var msg = ar.AsyncState as INetMsg;
            try
            {
                var stream = m_TcpClient.GetStream();
                stream.EndWrite(ar);
                if (m_Event != null)
                {
                    m_Event.OnMessageWrite(this, Msg);
                }
            }
            catch (Exception e)
            {
                onException(e);
            }

            if (msg != null)
            {
                msg.Recycle();
            }
        }

        private void onClosed()
        {
            Free();

            if (m_Event != null)
            {
                m_Event.OnMessageRead(this, null);
            }
        }

        private void onException(Exception exception)
        {
            LastError = exception;
            if (m_Event != null)
            {
                m_Event.OnException(this, exception);
            }

            if (OnException != null)
            {
                OnException.Invoke(exception);
            }

            if (Msg != null)
            {
                Msg.Recycle();
                Msg = null;
            }
            
            onClosed();
        }

        private void BeginRead()
        {
            var stream = m_TcpClient.GetStream();
            Assert.IsFalse(m_StartPos > m_EndPos);

            //循环使用buffer，将后边未读的片段前移
            if (m_StartPos < m_EndPos)
            {
                if (m_EndPos == m_ReadBuffer.Length)
                {
                    m_EndPos -= m_StartPos;
                    Array.Copy(m_ReadBuffer, m_StartPos, m_ReadBuffer, 0, m_EndPos);
                    m_StartPos = 0;
                }
            }
            else
            {
                m_StartPos = m_EndPos = 0;
            }

            stream.BeginRead(m_ReadBuffer, m_StartPos, m_ReadBuffer.Length - m_EndPos, m_TcpReadStreamCallback, this);
        }

        private static AsyncCallback m_TcpConnectCallback = ar =>
        {
            var session = ar.AsyncState as NetSession;
            if (session != null)
            {
                session.onConnected(ar);
            }
        };

        private static AsyncCallback m_TcpReadStreamCallback = ar =>
        {
            var session = ar.AsyncState as NetSession;
            if (session != null)
            {
                session.onReadStream(ar);
            }
        };

        public NetSession(CreateNetMsgDelegate createNetMsgDelegate)
        {
            Assert.IsNotNull(createNetMsgDelegate);
            
            this.m_MsgCreator = createNetMsgDelegate;
            this.m_WriteCallback = new AsyncCallback(onWriteStream);
        }
        
        public void Connect(string host, int port, AddressFamily addressFamily, INetSessionEvent netSessionEvent)
        {
            if (m_TcpClient != null)
            {
                m_TcpClient.Close();
                m_TcpClient = null;
            }

            try
            {
                bool noDelay = true;
#if UNITY_EDITOR || UNITY_STANDALONE
                const string TCP_SETTINGS = "tcp-settings.txt";
                if (System.IO.File.Exists(TCP_SETTINGS))
                {
                    string cfg = System.IO.File.ReadAllText(TCP_SETTINGS);
                    var js = TinyJSON.JSON.Load(cfg);
                    noDelay = js["nodelay"];
                }
#endif
                m_TcpClient = new TcpClient(addressFamily) {NoDelay = noDelay};
                m_TcpClient.Client.NoDelay = noDelay;
                m_TcpClient.BeginConnect(host, port, m_TcpConnectCallback, this);
                Connecting = true;
            }
            catch (Exception e)
            {
                onException(e);
            }
        }

        public bool Send(INetMsg msg)
        {
            var stream = m_TcpClient.GetStream();
            if (stream != null && stream.CanWrite)
            {
                try
                {
                    msg.Serialize();
                    stream.BeginWrite(msg.data, 0, msg.size, m_WriteCallback, msg);
                }
                catch (Exception e)
                {
                    msg.Recycle();
                    onException(e);
                    return false;
                }
            }

            return true;
        }

        public void Free()
        {
            if (m_TcpClient != null)
            {
                m_TcpClient.Close();
                m_TcpClient = null;
            }
        }
    }
}