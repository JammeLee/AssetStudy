using System.Net.Sockets;

namespace MFrame.NetEngine
{
    public interface INetSessionEvent
    {
        void OnConnected(INetSession session);

        void OnMessageRead(INetSession session, INetMsg msg);

        void OnMessageWrite(INetSession session, INetMsg msg);

        void OnException(INetSession session, System.Exception exception);
    }
    
    public interface INetSession
    {
        bool Connecting { get; }
        
        bool Connected { get; }

        System.Exception LastError { get; }
        
        int Latency { get; }
        
        System.Action<System.Exception> OnException { get; }
        
        INetMsg Msg { get; }

        void Connect(string host, int port, AddressFamily addressFamily, INetSessionEvent netSessionEvent);

        bool Send(INetMsg msg);

        void Free();
    }
}