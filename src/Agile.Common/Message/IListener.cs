namespace Agile.Common.Message
{
    /// <summary>
    /// 消息监听器
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="message">消息实体</param>
        void Handle(object message);
    }
}
