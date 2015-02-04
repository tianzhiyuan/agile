using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Logging
{
	/// <summary>
	/// logger factory
	/// </summary>
	public interface ILoggerFactory
	{
		/// <summary>
		/// create a logger with the given name
		/// </summary>
		/// <param name="name">logger name</param>
		/// <returns>ILogger instance</returns>
		ILogger Create(string name);
		/// <summary>
		/// create a logger with the given type
		/// </summary>
		/// <param name="type">type</param>
		/// <returns>ILogger instance</returns>
		ILogger Create(Type type);
	}
}
