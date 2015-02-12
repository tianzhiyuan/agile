using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Email
{
	/// <summary>
	/// email template renderder
	/// </summary>
	public interface ITemplateRenderer
	{
		string Parse<T>(string template, T model, bool isHtml = true);
	}
}
