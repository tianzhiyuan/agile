using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Security;
using RazorEngine.Templating;
namespace Agile.Framework.Email
{
    public class UnsafeRazorRenderer : ITemplateRenderer
    {
        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            return RazorEngine.Engine.Razor.RunCompile(template, template.Hash(), model?.GetType(), model);
        }

        public string Parse(string template, object model)
        {
            return RazorEngine.Engine.Razor.RunCompile(template, template.Hash(), model?.GetType(), model);
        }
    }
}
