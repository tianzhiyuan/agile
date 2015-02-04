using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agile.Framework.Tasks.Impl
{
    /// <summary>
    /// 每天触发一次
    /// </summary>
    public class DailyTrigger : TimerTrigger
    {
        private readonly int _hour;
        private readonly int _minite;
        private DateTime? _nextFireTimeUtc = null;
        /// <summary>
        /// 默认构造器 在每天的 hour:minite执行
        /// 精度不是很高，运行一段时间后会稍微有误差
        /// </summary>
        /// <param name="hour">Hour(Local)</param>
        /// <param name="minite">Minute(Local)</param>
        public DailyTrigger(int hour, int minite)
            :base()
        {
            _hour = hour;
            _minite = minite;
        }

        public override DateTime? GetNextFireTimeUtc()
        {
            return _nextFireTimeUtc;
        }


        public override void Enable()
        {
            var current = DateTime.UtcNow;

            
            var nextRun = new DateTime(
                current.Year,
                current.Month,
                current.Day,
                _hour,
                _minite,
                0).ToUniversalTime();
            //将当前时间往前调1分钟，消除分钟之后精度
            if (current.AddMinutes(-1) > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }
            TimeSpan due = nextRun - current;
            _timer.Change(due, new TimeSpan(1, 0, 0, 0));
            base.Enable();
        }

        public override void Disable()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            base.Disable();
        }

        protected override void OnTimerFired(object state)
        {
            _nextFireTimeUtc = DateTime.UtcNow.AddDays(1);
            base.OnTimerFired(state);
        }
    }
}
