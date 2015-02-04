using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Logging
{
	/// <summary>
	/// represents an empty logger factory
	/// </summary>
	public class EmptyLoggerFactory : ILoggerFactory
	{
		private static readonly ILogger _emptyLogger = new EmptyLogger();
		public ILogger Create(string name)
		{
			return _emptyLogger;
		}

		public ILogger Create(Type type)
		{
			return _emptyLogger;
		}
	}
}
