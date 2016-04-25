using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Components;

namespace Agile.Common.Logging
{
    public static class LoggerFactory
    {
        /// <summary>
        /// create a logger with the given name
        /// </summary>
        /// <param name="name">logger name</param>
        /// <returns>ILogger instance</returns>
        public static ILogger Create(string name)
        {
            return Factory.Create(name);
        }

        /// <summary>
        /// create a logger with the given type
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>ILogger instance</returns>
        public static ILogger Create(Type type)
        {
            return Factory.Create(type);
        }

        private static ILoggerFactory _factory;

        private static ILoggerFactory Factory => _factory ?? (_factory = ObjectContainer.Resolve<ILoggerFactory>());
    }
}
