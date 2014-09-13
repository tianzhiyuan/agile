using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agile.Common.Configurations;
using Agile.Common.Logging;

namespace Agile.Log4Net
{
    public static class ConfigurationExtensions
    {
        public static Configuration UseLog4Net(this Configuration configuration)
        {
            return UseLog4Net(configuration, "log4net.config");
        }
        public static Configuration UseLog4Net(this Configuration configuration, string configFile)
        {
            configuration.SetDefault<ILogger, Log4NetLogger>(new Log4NetLogger(configFile));
            return configuration;
        }
    }
}
