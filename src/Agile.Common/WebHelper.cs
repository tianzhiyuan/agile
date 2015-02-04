using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Agile.Common
{
	public class WebHelper
	{
		public static string MapPath(string path)
		{
			if (HostingEnvironment.IsHosted)
			{
				return HostingEnvironment.MapPath(path);
			}
			else
			{
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
				return Path.Combine(baseDirectory, path);
			}
		}
	}
}
