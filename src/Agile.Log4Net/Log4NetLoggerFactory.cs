using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Logging;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace Agile.Log4Net
{
	public class Log4NetLoggerFactory : ILoggerFactory
	{
		public Log4NetLoggerFactory(string configFile)
		{
			var file = new FileInfo(configFile);
			if (!file.Exists)
			{
				file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile));
			}

			if (file.Exists)
			{
				XmlConfigurator.ConfigureAndWatch(file);
			}
			else
			{
				BasicConfigurator.Configure(new ConsoleAppender { Layout = new PatternLayout() });
			}
		}
		/// <summary>Create a new Log4NetLogger instance.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ILogger Create(string name)
		{
			return new Log4NetLogger(LogManager.GetLogger(name));
		}
		/// <summary>Create a new Log4NetLogger instance.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ILogger Create(Type type)
		{
			return new Log4NetLogger(LogManager.GetLogger(type));
		}
	}
}
