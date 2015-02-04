using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Tasks.Impl
{
    /// <summary>
    /// 基本任务，可以执行一个或多个action
    /// </summary>
    public class SimpleTask : AbstractTask
    {
        private readonly IList<Action> _actions;

        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="actions">待执行的Action</param>
        public SimpleTask(params Action[] actions)
            : base()
        {
            _actions = actions ?? new Action[0];
        }

        public override void Execute(TaskExecutionContext context)
        {
            foreach (var action in _actions)
            {
                action.Invoke();
            }
        }
        public void AddAction(Action action)
        {
            _actions.Add(action);
        }
        public void RemoveAction(Action action)
        {
            _actions.Remove(action);
        }
    }
}
