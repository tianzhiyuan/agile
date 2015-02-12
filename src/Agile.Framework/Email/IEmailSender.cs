using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Email
{
	/// <summary>
	/// send email
	/// </summary>
	public interface IEmailSender
	{
		void Send(Email email);
		Task SendAsync(Email email);
	}
}
