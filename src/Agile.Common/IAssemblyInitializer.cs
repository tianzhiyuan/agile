using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Agile.Common
{
    public interface IAssemblyInitializer
    {
        void Initialize(Assembly[] assemblies);
    }
}
