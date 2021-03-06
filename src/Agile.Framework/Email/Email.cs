﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agile.Framework.Email
{
	/// <summary>
	/// inspired by FluentEmail
	/// see github https://github.com/lukencode/FluentEmail
	/// seperate email construction and sending for we usually use EDM to send an email.
	/// </summary>
	public class Email : IDisposable
	{
		private bool? _useSsl;
		private bool _bodyIsHtml = true;
		private ITemplateRenderer _renderer;

		public MailMessage Message { get; private set; }

		

		/// <summary>
		/// Creates a new email instance using the default from
		/// address from smtp config settings
		/// </summary>
		private Email(){ }

		public bool UseSsl { get { return _useSsl ?? true; } }
		/// <summary>
		/// Creates a new email instance using the default from
		/// address from smtp config settings
		/// </summary>
		/// <returns>Instance of the Email class</returns>
		public static Email FromDefault()
		{
			var email = new Email
			{
				Message = new MailMessage()
			};

			return email;
		}

		/// <summary>
		/// Creates a new Email instance and sets the from
		/// property
		/// </summary>
		/// <param name="emailAddress">Email address to send from</param>
		/// <param name="name">Name to send from</param>
		/// <returns>Instance of the Email class</returns>
		public static Email From(string emailAddress, string name = "")
		{
			var email = new Email
			{
				Message = { From = new MailAddress(emailAddress, name) }
			};
			return email;
		}

		/// <summary>
		/// Adds a reciepient to the email, Splits name and address on ';'
		/// </summary>
		/// <param name="emailAddress">Email address of recipeient</param>
		/// <param name="name">Name of recipient</param>
		/// <returns>Instance of the Email class</returns>
		public Email To(string emailAddress, string name)
		{
			if (emailAddress.Contains(";"))
			{
				//email address has semi-colon, try split
				var nameSplit = name.Split(';');
				var addressSplit = emailAddress.Split(';');
				for (int i = 0; i < addressSplit.Length; i++)
				{
					var currentName = string.Empty;
					if ((nameSplit.Length - 1) >= i)
					{
						currentName = nameSplit[i];
					}
					Message.To.Add(new MailAddress(addressSplit[i], currentName));
				}
			}
			else
			{
				Message.To.Add(new MailAddress(emailAddress, name));
			}
			return this;
		}

		/// <summary>
		/// Adds a reciepient to the email
		/// </summary>
		/// <param name="emailAddress">Email address of recipeient (allows multiple splitting on ';')</param>
		/// <returns></returns>
		public Email To(string emailAddress)
		{
			if (emailAddress.Contains(";"))
			{
				foreach (string address in emailAddress.Split(';'))
				{
					Message.To.Add(new MailAddress(address));
				}
			}
			else
			{
				Message.To.Add(new MailAddress(emailAddress));
			}

			return this;
		}

		/// <summary>
		/// Adds all reciepients in list to email
		/// </summary>
		/// <param name="mailAddresses">List of recipients</param>
		/// <returns>Instance of the Email class</returns>
		public Email To(IList<MailAddress> mailAddresses)
		{
			foreach (var address in mailAddresses)
			{
				Message.To.Add(address);
			}
			return this;
		}

		/// <summary>
		/// Adds a Carbon Copy to the email
		/// </summary>
		/// <param name="emailAddress">Email address to cc</param>
		/// <param name="name">Name to cc</param>
		/// <returns>Instance of the Email class</returns>
		public Email CC(string emailAddress, string name = "")
		{
			Message.CC.Add(new MailAddress(emailAddress, name));
			return this;
		}

		/// <summary>
		/// Adds all Carbon Copy in list to an email
		/// </summary>
		/// <param name="mailAddresses">List of recipients to CC</param>
		/// <returns>Instance of the Email class</returns>
		public Email CC(IList<MailAddress> mailAddresses)
		{
			foreach (var address in mailAddresses)
			{
				Message.CC.Add(address);
			}
			return this;
		}

		/// <summary>
		/// Adds a blind carbon copy to the email
		/// </summary>
		/// <param name="emailAddress">Email address of bcc</param>
		/// <param name="name">Name of bcc</param>
		/// <returns>Instance of the Email class</returns>
		public Email BCC(string emailAddress, string name = "")
		{
			Message.Bcc.Add(new MailAddress(emailAddress, name));
			return this;
		}

		/// <summary>
		/// Adds all blind carbon copy in list to an email
		/// </summary>
		/// <param name="mailAddresses">List of recipients to BCC</param>
		/// <returns>Instance of the Email class</returns>
		public Email BCC(IList<MailAddress> mailAddresses)
		{
			foreach (var address in mailAddresses)
			{
				Message.Bcc.Add(address);
			}
			return this;
		}

		/// <summary>
		/// Sets the ReplyTo address on the email
		/// </summary>
		/// <param name="address">The ReplyTo Address</param>
		/// <returns></returns>
		public Email ReplyTo(string address)
		{
			Message.ReplyToList.Add(new MailAddress(address));

			return this;
		}

		/// <summary>
		/// Sets the ReplyTo address on the email
		/// </summary>
		/// <param name="address">The ReplyTo Address</param>
		/// <param name="name">The Display Name of the ReplyTo</param>
		/// <returns></returns>
		public Email ReplyTo(string address, string name)
		{
			Message.ReplyToList.Add(new MailAddress(address, name));

			return this;
		}

		/// <summary>
		/// Sets the subject of the email
		/// </summary>
		/// <param name="subject">email subject</param>
		/// <returns>Instance of the Email class</returns>
		public Email Subject(string subject)
		{
			Message.Subject = subject;
			return this;
		}

		/// <summary>
		/// Adds a Body to the Email
		/// </summary>
		/// <param name="body">The content of the body</param>
		public Email Body(string body)
		{
			Message.Body = body;
			return this;
		}

		/// <summary>
		/// Marks the email as High Priority
		/// </summary>
		public Email HighPriority()
		{
			Message.Priority = MailPriority.High;
			return this;
		}

		/// <summary>
		/// Marks the email as Low Priority
		/// </summary>
		public Email LowPriority()
		{
			Message.Priority = MailPriority.Low;
			return this;
		}

		/// <summary>
		/// Set the template rendering engine to use, defaults to RazorEngine
		/// </summary>
		public Email UsingTemplateEngine(ITemplateRenderer renderer)
		{
			_renderer = renderer;
			return this;
		}

		/// <summary>
		/// Adds template to email from embedded resource
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">Path the the embedded resource eg [YourAssembly].[YourResourceFolder].[YourFilename.txt]</param>
		/// <param name="model">Model for the template</param>
		/// <param name="assembly">The assembly your resource is in. Defaults to calling assembly.</param>
		/// <returns></returns>
		public Email UsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly = null)
		{
			CheckRenderer();

			assembly = assembly ?? Assembly.GetCallingAssembly();

			var template = GetResourceAsString(assembly, path);
			var result = _renderer.Parse(template, model, _bodyIsHtml);
			Message.Body = result;
			Message.IsBodyHtml = _bodyIsHtml;

			return this;
		}

		/// <summary>
		/// Adds the template file to the email
		/// </summary>
		/// <param name="filename">The path to the file to load</param>
		/// <param name="model"></param>
		/// <returns>Instance of the Email class</returns>
		public Email UsingTemplateFromFile<T>(string filename, T model)
		{
			var path = WebHelper.MapPath(filename);


			string template = System.IO.File.ReadAllText(path);

			CheckRenderer();

			var result = _renderer.Parse(template, model, _bodyIsHtml);
			Message.Body = result;
			Message.IsBodyHtml = _bodyIsHtml;

			return this;
		}

		/// <summary>
		/// Adds a culture specific template file to the email
		/// </summary>
		/// <param name="filename">The path to the file to load</param>
		/// /// <param name="model">The razor model</param>
		/// <param name="culture">The culture of the template (Default is the current culture)</param>
		/// <returns>Instance of the Email class</returns>
		public Email UsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture = null)
		{
			var wantedCulture = culture ?? Thread.CurrentThread.CurrentUICulture;
			var cultureFile = GetCultureFileName(filename, wantedCulture);
			return UsingTemplateFromFile(cultureFile, model);
		}

		/// <summary>
		/// Adds razor template to the email
		/// </summary>
		/// <param name="template">The razor template</param>
		/// <param name="model"></param>
		/// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
		/// <returns>Instance of the Email class</returns>
		public Email UsingTemplate<T>(string template, T model, bool isHtml = true)
		{
			CheckRenderer();

			var result = _renderer.Parse(template, model, isHtml);
			Message.Body = result;
			Message.IsBodyHtml = isHtml;

			return this;
		}

		/// <summary>
		/// Adds an Attachment to the Email
		/// </summary>
		/// <param name="attachment">The Attachment to add</param>
		/// <returns>Instance of the Email class</returns>
		public Email Attach(Attachment attachment)
		{
			if (!Message.Attachments.Contains(attachment))
				Message.Attachments.Add(attachment);

			return this;
		}

		/// <summary>
		/// Adds Multiple Attachments to the Email
		/// </summary>
		/// <param name="attachments">The List of Attachments to add</param>
		/// <returns>Instance of the Email class</returns>
		public Email Attach(IEnumerable<Attachment> attachments)
		{
			foreach (var attachment in attachments.Where(attachment => !Message.Attachments.Contains(attachment)))
			{
				Message.Attachments.Add(attachment);
			}
			return this;
		}

		

		public Email UseSSL()
		{
			_useSsl = true;
			return this;
		}

		/// <summary>
		/// Sets Message to html (set by default)
		/// </summary>
		public Email BodyAsHtml()
		{
			_bodyIsHtml = true;
			return this;
		}

		/// <summary>
		/// Sets Message to plain text (set by default)
		/// </summary>
		public Email BodyAsPlainText()
		{
			_bodyIsHtml = false;
			return this;
		}

		


		


		private void CheckRenderer()
		{
			if (_renderer != null) return;


			//_renderer = new RazorRenderer();

		}

		

		private static string GetCultureFileName(string fileName, CultureInfo culture)
		{
			var fullFilePath = WebHelper.MapPath(fileName);
			var extension = Path.GetExtension(fullFilePath);
			var cultureExtension = string.Format("{0}{1}", culture.Name, extension);

			var cultureFile = Path.ChangeExtension(fullFilePath, cultureExtension);
			if (System.IO.File.Exists(cultureFile))
				return cultureFile;
			else
				return fullFilePath;
		}

		internal static string GetResourceAsString(Assembly assembly, string path)
		{
			string result;

			using (var stream = assembly.GetManifestResourceStream(path))
			using (var reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}

		public void Dispose()
		{
			if (Message != null)
			{
				Message.Dispose();
			}
		}
	}
}
