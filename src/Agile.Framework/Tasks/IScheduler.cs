using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Tasks
{
    /// <summary>
    /// 任务计划
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// 将任务加入队列
        /// 如果当前状态为running，加入队列后trigger会立刻启用
        /// 如果为not running，则在调用start之后启用trigger
        /// </summary>
        /// <param name="task"></param>
        /// <param name="trigger"></param>
        void ScheduleTask(ITask task, ITrigger trigger);
        /// <summary>
        /// 关闭任务计划
        /// </summary>
        /// <param name="waitForTaskComplete">是否等待当前任务执行完成</param>
        void Shutdown(bool waitForTaskComplete);
        /// <summary>
        /// 是否在执行
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// 启动任务计划
        /// </summary>
        void Start();
    }
}
