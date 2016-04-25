using Agile.Common;
using Agile.Common.Components;
using Autofac;

namespace Agile.Autofac
{
    public static class ConfigurationExtensitions
    {
        public static WireUp UseAutofac(this WireUp configuration)
        {
            return UseAutofac(configuration, new ContainerBuilder());
        }
        /// <summary>Use Autofac as the object container.
        /// </summary>
        /// <returns></returns>
        public static WireUp UseAutofac(this WireUp configuration, ContainerBuilder containerBuilder)
        {
            ObjectContainer.SetContainer(new AutofacContainer(containerBuilder));
            return configuration;
        }
    }
}
