using System;
using System.Threading;
using Agile.Framework.Properties;


namespace Agile.Framework.Tasks.Impl
{
    /// <summary>
    /// 可触发一次或多次的触发器
    /// </summary>
    public class RepeatTrigger : TimerTrigger
    {
        /// <summary>
        /// 指示无限次触发
        /// </summary>
        public const int RepeatInfinite = -1;
        /// <summary>
        /// 一共触发多少次
        /// 最少一次，<see cref="RepeatInfinite"/>表示无限重复，直到<see cref="RepeatUntil"/>（如果不为空）
        /// 默认为1
        /// </summary>
        public int RepeatCount { get { return _repeatCount; } }
        /// <summary>
        /// 触发间隔
        /// </summary>
        public TimeSpan RepeatInterval { get; private set; }
        /// <summary>
        /// 触发起始时间，默认为当前时间
        /// </summary>
        public DateTime StartTimeUtc { get; private set; }
        /// <summary>
        /// 触发终止时间，null表示不指定
        /// </summary>
        public DateTime? RepeatUntil { get { return _repeatUntil; } }
        #region constructors
        /// <summary>
        /// 默认立即执行一次的触发器，在加入<see cref="IScheduler"/>后才会执行
        /// </summary>
        public RepeatTrigger()
            : base()
        {
            this.StartTimeUtc = DateTime.UtcNow;
            _nextFireTimeUtc = this.StartTimeUtc;
            this.RepeatInterval = new TimeSpan(100);
        }
        /// <summary>
        /// 在指定时间执行一次的触发器
        /// </summary>
        /// <param name="startTimeUtc">指定触发时间</param>
        public RepeatTrigger(DateTime startTimeUtc)
            : base()
        {
            this.StartTimeUtc = startTimeUtc;
            _nextFireTimeUtc = this.StartTimeUtc;
            this.RepeatInterval = new TimeSpan(100);
        }
        /// <summary>
        /// 以固定间隔触发，无限次
        /// </summary>
        /// <param name="interval">触发时间间隔</param>
        public RepeatTrigger(TimeSpan interval)
            : base()
        {
            this._repeatCount = RepeatInfinite;
            this.StartTimeUtc = DateTime.UtcNow;
            
            _nextFireTimeUtc = this.StartTimeUtc;
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentException(Resources.Trigger_IntervalLargerThanZero, "interval");
            }
            this.RepeatInterval = interval;
        }

        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <param name="repeatCount">触发次数</param>
        /// <param name="startTimeUtc">第一次执行时间</param>
        /// <param name="interval">触发间隔</param>
        /// <param name="repeatUntil">触发终止时间</param>
        public RepeatTrigger(
            int repeatCount,
            DateTime startTimeUtc,
            TimeSpan interval,
            DateTime? repeatUntil = null
            ) : base()
        {
            if (repeatCount < 1 && repeatCount != RepeatInfinite)
            {
                throw new ArgumentException(Resources.Trigger_RepeatCountNoLessThanOne, "repeatCount");
            }
            this.Enabled = false;
            this._repeatCount = repeatCount;
            this.StartTimeUtc = startTimeUtc;
            if (interval < TimeSpan.Zero)
            {
                throw new ArgumentException(Resources.Trigger_IntervalLargerThanZero, "interval");
            }
            this.RepeatInterval = interval;
            _nextFireTimeUtc = this.StartTimeUtc;
            this._repeatUntil = repeatUntil;
            if (repeatUntil != null)
            {
                if (repeatUntil < DateTime.UtcNow)
                {
                    throw new ArgumentException(Resources.Trigger_EndTimeEarlyThanCurrent, "repeatUntil");
                }

                if (repeatUntil < startTimeUtc)
                {
                    throw new ArgumentException(Resources.Trigger_EndTimeEarlyThanStartTime, "repeatUntil");
                }
            }
        }

        #endregion
        #region overrides
        public override DateTime? GetNextFireTimeUtc()
        {
            return HasRunnout() ? null : _nextFireTimeUtc;
        }

        public override void Enable()
        {
            var currentTime = DateTime.UtcNow;
            TimeSpan due;
            if (currentTime < this.StartTimeUtc)
            {
                due = StartTimeUtc - currentTime;
            }
            else
            {
                due = new TimeSpan(0, 0, 0, 0, 100);
            }
            if (HasRunnout()) return;
            var interval = this.RepeatInterval;
            _timer.Change(due, interval);
            base.Enable();
        }

        public override void Disable()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            base.Disable();
        }

        protected override void OnTimerFired(object state)
        {
            if (HasRunnout())
            {
                return;
            }
            _nextFireTimeUtc = DateTime.UtcNow + this.RepeatInterval;
            base.OnTimerFired(state);
        }
        #endregion

        #region private members
        private DateTime? _nextFireTimeUtc;
        private readonly int _repeatCount = 1;
        private readonly DateTime? _repeatUntil = null;
        /// <summary>
        /// 是否已经不可以继续触发
        /// </summary>
        /// <returns>true-不可以继续触发 false-可以继续触发</returns>
        private bool HasRunnout()
        {
            if (TriggeredCount == this.RepeatCount)
            {
                this.Disable();
                return true;
            }
            if (this.RepeatUntil != null)
            {
                if (this.RepeatUntil.Value < DateTime.UtcNow)
                {
                    this.Disable();
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
