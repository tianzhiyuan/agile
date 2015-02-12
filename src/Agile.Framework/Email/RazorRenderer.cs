using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Security;
using RazorEngine;
using RazorEngine.Templating;
namespace Agile.Framework.Email
{
	public class RazorRenderer : ITemplateRenderer
	{
		public string Parse<T>(string template, T model, bool isHtml = true)
		{
			return Engine.IsolatedRazor.RunCompile(template, template.Hash(), null, model);
		}
	}
}
