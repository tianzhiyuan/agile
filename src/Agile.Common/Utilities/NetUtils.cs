using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Logging;

namespace Agile.Common.Utilities
{
	public class NetUtils
	{
		private readonly ILogger _logger;
		public NetUtils(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.Create(typeof (NetUtils));
		}
		public bool TryPing(string hostNameOrAddress, int timeout)
		{
			try
			{
				using (var ping = new Ping())
				{
					var reply = ping.Send(hostNameOrAddress, timeout);
					if (reply == null || reply.Status != IPStatus.Success)
					{
						return false;
					}
				}
			}
			catch (Exception error)
			{
				_logger.Error("", error);
				return false;
			}
			return true;
		}
	}
}
