using System;
using System.Threading;

namespace Agile.Framework.Tasks.Impl
{
    /// <summary>
    /// 基于定时器的触发器
    /// </summary>
    public abstract class TimerTrigger : AbstractTrigger, IDisposable
    {
        private DateTime? _lastFireTimeUtc;
        /// <summary>
        /// 定时器
        /// </summary>
        protected Timer _timer;
        /// <summary>
        /// 已触发次数
        /// </summary>
        public int TriggeredCount { get; protected set; }
        public override event EventHandler Triggered;
        protected TimerTrigger() : base()
        {
            TriggeredCount = 0;
            _timer = new Timer(this.OnTimerFired, null, Timeout.Infinite, Timeout.Infinite);
        }
        public override DateTime? GetLastFireTimeUtc()
        {
            return _lastFireTimeUtc;
        }
        
        protected virtual void OnTimerFired(object state)
        {
            _lastFireTimeUtc = DateTime.UtcNow;
            TriggeredCount++;
            if (this.Triggered != null)
            {
                Triggered.Invoke(this, EventArgs.Empty);
            }
        }
        
        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;
        }
    }
}
