using Agile.Common.Components;
using Agile.Common.Configurations;
using Autofac;

namespace Agile.Autofac
{
    public static class ConfigurationExtensitions
    {
        public static Configuration UseAutofac(this Configuration configuration)
        {
            return UseAutofac(configuration, new ContainerBuilder());
        }
        /// <summary>Use Autofac as the object container.
        /// </summary>
        /// <returns></returns>
        public static Configuration UseAutofac(this Configuration configuration, ContainerBuilder containerBuilder)
        {
            ObjectContainer.SetContainer(new AutofacContainer(containerBuilder));
            return configuration;
        }
    }
}
