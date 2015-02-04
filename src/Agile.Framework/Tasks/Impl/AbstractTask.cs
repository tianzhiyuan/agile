using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Utilities;


namespace Agile.Framework.Tasks.Impl
{
    public abstract class AbstractTask : ITask
    {
        protected AbstractTask()
        {
            this.Key = CombGuid.Generate();
            this.Reentrant = false;
            this.FriendlyName = this.GetType().FullName;
        }
        public Guid Key { get; protected set; }
        public bool Reentrant { get; set; }
        public string FriendlyName { get; set; }
        public abstract void Execute(TaskExecutionContext context);
    }
}
