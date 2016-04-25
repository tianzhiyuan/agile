using System;

namespace Agile.Common.Message
{
    /// <summary>
    /// 消息总线
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// 注册消息处理
        /// </summary>
        /// <param name="listener">监听器</param>
        /// <param name="messageType"></param>
        void Subscribe(IListener listener, Type messageType);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <param name="message">消息实体</param>
        void Post<TMessage>(TMessage message) where TMessage : class;
    }
}
