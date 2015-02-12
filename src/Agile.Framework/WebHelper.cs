using System;
using System.IO;
using System.Web.Hosting;

namespace Agile.Framework
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
