using LLY.Core.LiCai.Message;

namespace Agile.Common.Message
{
    /// <summary>
    /// 基础监听器
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class BaseListener<TMessage> : IListener where TMessage:class
    {


        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="message"></param>
        public abstract void DoHandle(TMessage message);
        void IListener.Handle(object message)
        {
            this.DoHandle((TMessage) message);
        }
    }
}
