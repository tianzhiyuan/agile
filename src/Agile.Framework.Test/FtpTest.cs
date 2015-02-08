using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Logging;
using Agile.Framework.File.Impl;
using NUnit.Framework;

namespace Agile.Framework.Test
{
	[TestFixture]
	public class FtpTest
	{
		[Test]
		public void Test()
		{
			var ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1");
			ftpRequest.Credentials = new NetworkCredential("sa", "111111");
			ftpRequest.UseBinary = true;
			ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
			using (var response = (FtpWebResponse)ftpRequest.GetResponse())
			using (var stream = new StreamReader(response.GetResponseStream()))
			{
				Console.WriteLine(stream.ReadToEnd());
			}
		}

		[Test]
		public void Test2()
		{
			var svc = new FtpFileService(new EmptyLoggerFactory(), null)
				{
					Username = "sa",
					Password = "111111",
					ServerAddr = "ftp://127.0.0.1"
				};
			var bytes = System.IO.File.ReadAllBytes(@"d:\test.jpg");
			svc.Create(bytes, "test.jpg");
		}
		[Test]
		public void Test3()
		{
			int[] obj = new int[]{1,2};
			Console.WriteLine(Convert.ToString(obj));

			var test = "1";
			Console.WriteLine(Convert.ChangeType(test, typeof (int)));
		}
	}
}
