using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Plugins
{
    public interface IPlugin
    {
        void Install();
        void Uninstall();
    }
}
