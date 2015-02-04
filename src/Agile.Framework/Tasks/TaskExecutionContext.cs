using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Framework.Tasks;

namespace Agile.Framework.Tasks
{
    /// <summary>
    /// 任务执行上下文
    /// </summary>
    public class TaskExecutionContext
    {
        private readonly IDictionary<string, object> items; 
        public IScheduler Scheduler { get; private set; }
        public ITrigger Trigger { get; private set; }
        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="scheduler">任务计划</param>
        /// <param name="trigger">触发器</param>
        public TaskExecutionContext(IScheduler scheduler, ITrigger trigger)
        {
            Scheduler = scheduler;
            Trigger = trigger;
            items = new Dictionary<string, object>();
        }

        public object Get(string key)
        {
            object value;
            items.TryGetValue(key, out value);
            return value;
        }
        public void Put(string key, object value)
        {
            if (items.ContainsKey(key))
            {
                items[key] = value;
            }
            items.Add(key, value);
        }
    }
}
