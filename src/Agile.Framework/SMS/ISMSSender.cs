using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.SMS
{
	public interface ISMSSender
	{
		void Send(SMSMessage smsMessage);
		Task SendAsync(SMSMessage smsMessage);
	}
}
