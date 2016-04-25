using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Components;
using Agile.Common.Logging;
using Agile.Common.Serialization;

namespace Agile.Common
{
    public class WireUp
    {
        private WireUp()
        {
        }

        public static WireUp Instance { get; } = new WireUp();

        public WireUp RegisterCommon()
        {
            ObjectContainer.Register<IJsonSerializer, Json>();

            return this;
        }

        public WireUp RegisterBusinessComponents(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {

                        var component = type.GetCustomAttribute<ComponentAttribute>();
                        if (component != null)
                        {
                            var interfaces = type.GetInterfaces();
                            if (interfaces.Any())
                            {
                                foreach (var interfaceType in interfaces)
                                {
                                    ObjectContainer.RegisterType(interfaceType, type, component.LifeStyle);
                                }
                            }
                            else
                            {
                                ObjectContainer.RegisterType(type);
                            }
                        }
                    }
                }
                catch (System.Exception error)
                {
                    System.Diagnostics.Trace.WriteLine(error);
                }
            }
            return this;
        }

        public WireUp Initialize(params Assembly[] assemblies)
        {
            foreach (var init in ObjectContainer.ResolveAll<IAssemblyInitializer>())
            {
                init.Initialize(assemblies);
            }
            return this;
        }
    }
}
