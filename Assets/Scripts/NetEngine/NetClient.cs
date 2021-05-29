using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MFrame.NetEngine
{
    public class NetClient : INetSessionEvent
    {
        #region Private

        private INetSession _nowSession;

        private Queue<INetMsg> _receiveQueue;
        
        private string mErr;

        #endregion

        #region Public

        /// <summary>
        /// 创建新的网络会话，每次进行连接都需要重新创建
        /// </summary>
        public static Func<INetSession> SessionCreator;

        /// <summary>
        /// 创建一个消息类，用于存储消息
        /// </summary>
        public static Func<int, int, INetMsg> NetMsgCreator;

        public delegate void ConnectedHandler(NetClient client);

        public delegate void DisconnectedHandler(NetClient client);

        public ConnectedHandler onConnected;

        public DisconnectedHandler onDisconnected;

        public Action<string> ErrorLogger;
        
        public string Error
        {
            get { return mErr; }
            private set { mErr = value.Trim();
                if (ErrorLogger != null)
                {
                    ErrorLogger(mErr);
                }
            }
        }
        
        /// <summary>
        /// 是否已建立连接
        /// </summary>
        public bool Connected
        {
            get { return _nowSession != null && _nowSession.Connected; }
        }
        
        /// <summary>
        /// 是否正在建立连接
        /// </summary>
        public bool Connecting
        {
            get { return _nowSession != null && _nowSession.Connecting; }
        }

        /// <summary>
        /// 延迟
        /// </summary>
        public int Latency
        {
            get { return _nowSession != null ? _nowSession.Latency : 0; }
        }
        
        public static bool IsIPv6 { get; private set; }
        
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiveQueue">接受消息的队列</param>
        /// <param name="onConnected">连接回调</param>
        /// <param name="onDisconnected">断开连接回调</param>
        public NetClient(Queue<INetMsg> receiveQueue, ConnectedHandler onConnected, DisconnectedHandler onDisconnected)
            : this(receiveQueue, onConnected, onDisconnected, null)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiveQueue">接受消息的队列</param>
        /// <param name="onConnected">连接回调</param>
        /// <param name="onDisconnected">断开连接回调</param>
        /// <param name="errorLogger">错误回调</param>
        public NetClient(Queue<INetMsg> receiveQueue, ConnectedHandler onConnected, DisconnectedHandler onDisconnected,
            Action<string> errorLogger)
        {
            this._receiveQueue = receiveQueue;
            this.onConnected = onConnected;
            this.onDisconnected = onDisconnected;
            this.ErrorLogger = errorLogger;
        }

        public void Connect(string host, int port, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            if (string.IsNullOrEmpty(host) || port < 1)
            {
                return;
            }

            mErr = null;
            if (_nowSession != null)
            {
                _nowSession.Free();
            }

            _nowSession = SessionCreator.Invoke();
            _nowSession.Connect(host, port, addressFamily, this);
        }
        
        public void OnConnected(INetSession session)
        {
            if (session != _nowSession)
            {
                session.Free();
                return;
            }

            if (session.Connected)
            {
                if (onConnected != null)
                {
                    onConnected(this);
                }
            }
        }

        public void OnMessageRead(INetSession session, INetMsg msg)
        {
            if (session != _nowSession)
            {
                session.Free();
                return;
            }

            //会话未连接
            if (!session.Connected)
            {
                //抛出错误信息
                if (session.LastError != null)
                {
                    Error = session.LastError.ToString();
                }

                if (onDisconnected != null)
                {
                    onDisconnected(this);
                }

                return;
            }

            //消息入队
            _receiveQueue.Enqueue(msg);
        }

        public void OnMessageWrite(INetSession session, INetMsg msg)
        {
            
        }

        public void OnException(INetSession session, Exception exception)
        {
            Error = exception.Message;
        }

        public void UnpackMsgs(INetMsgHandler netMsgHandler)
        {
            while (_receiveQueue != null && _receiveQueue.Count > 0)
            {
                var read = false;
                var msg = _receiveQueue.Dequeue();
                if (msg != null)
                {
                    netMsgHandler.TryHandler(msg);
                    msg.Recycle();
                }
            }
        }

        public void Close()
        {
            try
            {
                if (_nowSession != null)
                {
                    _nowSession.Free();
                }

                _nowSession = null;
            }
            finally{}

            if (onDisconnected != null)
            {
                onDisconnected(this);
            }
        }

        public bool Send(INetMsg msg)
        {
            if (_nowSession == null)
            {
                return false;
            }
            return _nowSession.Send(msg);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", base.ToString(),
                _nowSession != null && _nowSession.Connected ? "Connected" : "Disconnected");
        }

        public static string RefreshAddressFamily(string host)
        {
            var IPs = Dns.GetHostAddresses(host);
            if (IPs != null && IPs.Length > 0)
            {
                IsIPv6 = IPs[0].AddressFamily == AddressFamily.InterNetworkV6;
                return IPs[0].ToString();
            }

            return null;
        }
    }
}