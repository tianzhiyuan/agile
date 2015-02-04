using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agile.Common.Components;
using Agile.Common.Configurations;
using Agile.Common.Logging;

namespace Agile.Log4Net
{
    public static class ConfigurationExtensions
    {
		public static Configuration UseLog4Net(this Configuration configuration, string configFile)
		{
			ObjectContainer.Current.RegisterInstance<ILoggerFactory, Log4NetLoggerFactory>(
				new Log4NetLoggerFactory(configFile));
			return configuration;
		}
		public static Configuration UseLog4Net(this Configuration configuration)
		{
			ObjectContainer.Current.RegisterInstance<ILoggerFactory, Log4NetLoggerFactory>(
				new Log4NetLoggerFactory("log4net.config"));
			return configuration;
		}
    }
}
