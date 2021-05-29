namespace MFrame.NetEngine
{
    public interface INetMsg
    {
        /// <summary>
        /// 消息类型（协议号）
        /// </summary>
        int type { get; }
        
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        byte[] data { get; }
        
        /// <summary>
        /// 包体大小
        /// </summary>
        int size { get; }
        
        /// <summary>
        /// 消息体大小
        /// </summary>
        int bodySize { get; }

        int ReadBuffer(byte[] buffer, int offset, int length);

        bool IsComplete();

        INetMsg WriteBuffer(byte[] buffer, int offset, int length);

        INetMsg WriteBuffer(System.IntPtr ptr, int len);

        /// <summary>
        /// 消息发送前，序列化
        /// </summary>
        void Serialize();

        /// <summary>
        /// 接受消息后，反序列化
        /// </summary>
        void Deserialize();

        /// <summary>
        /// 回收：对象池管理
        /// </summary>
        void Recycle();
    }

    public interface INetMsgHandler
    {
        /// <summary>
        /// 尝试解析消息
        /// </summary>
        /// <param name="msg">消息对象</param>
        /// <returns>是否成功解析</returns>
        bool TryHandler(INetMsg msg);
    }
}