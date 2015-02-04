using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Utilities;

namespace Agile.Framework.Tasks.Impl
{
    public abstract class AbstractTrigger : ITrigger
    {
        protected AbstractTrigger()
        {
            this.Key = CombGuid.Generate();
            this.Enabled = false;
        }

        public abstract event EventHandler Triggered;
        public Guid Key { get; protected set; }
        public virtual string Name { get; set; }
        public abstract DateTime? GetLastFireTimeUtc();

        public abstract DateTime? GetNextFireTimeUtc();

        public bool Enabled { get; protected set; }
        public virtual void Enable()
        {
            this.Enabled = true;
        }

        public virtual void Disable()
        {
            this.Enabled = false;
        }
    }
}
