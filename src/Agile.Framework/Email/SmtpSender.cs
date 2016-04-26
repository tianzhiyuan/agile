using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Email
{
	/// <summary>
	/// simple smtp email sender
	/// </summary>
	public class SmtpSender : IEmailSender, IDisposable
	{
		private SmtpClient _client;
		public SmtpSender()
		{
			_client = new SmtpClient();
		}
		public SmtpSender(string host, int port)
		{
			_client = new SmtpClient(host, port);
		}
		public void Send(Email email)
		{
			_client.EnableSsl = email.UseSsl;
			_client.Send(email.Message);
		}

		public void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
			}
		}
	}
}
