using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Tasks
{
    /// <summary>
    /// 任务执行参数
    /// </summary>
    public class TaskExecutionArgs : EventArgs
    {
        /// <summary>
        /// 任务键
        /// </summary>
        public Guid TaskKey { get; private set; }
        /// <summary>
        /// 触发器键
        /// </summary>
        public Guid TriggerKey { get; private set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; private set; }
        /// <summary>
        /// 触发器名称
        /// </summary>
        public string TriggerName { get; private set; }
        /// <summary>
        /// 任务执行开始UTC时间
        /// </summary>
        public DateTime StartTimeUtc { get; private set; }
        /// <summary>
        /// 任务执行结束UTC时间
        /// </summary>
        public DateTime? EndTimeUtc { get; private set; }
        /// <summary>
        /// 任务执行时错误
        /// </summary>
        public Exception LastError { get; private set; }

        public TaskExecutionArgs(
            Guid taskKey,
            Guid triggerKey,
            string taskName,
            string triggerName,
            DateTime startTimeUtc,
            DateTime? endTimeUtc = null,
            Exception lastError = null
            )
        {
            this.TaskKey = taskKey;
            this.TriggerKey = triggerKey;
            this.TaskName = taskName;
            this.StartTimeUtc = startTimeUtc;
            this.TriggerName = triggerName;
            this.EndTimeUtc = endTimeUtc;
            this.LastError = lastError;
        }
    }
}
