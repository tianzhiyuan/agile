using Agile.Common;
using Agile.Common.Components;
using Agile.Common.Logging;

namespace Agile.Log4Net
{
    public static class ConfigurationExtensions
    {
		public static WireUp UseLog4Net(this WireUp configuration, string configFile = "log4net.config")
		{
			ObjectContainer.Current.RegisterInstance<ILoggerFactory, Log4NetLoggerFactory>(
				new Log4NetLoggerFactory(configFile));
			return configuration;
		}
    }
}
