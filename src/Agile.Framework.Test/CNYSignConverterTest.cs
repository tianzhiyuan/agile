using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Utils;
using NUnit.Framework;

namespace Agile.Framework.Test
{
	[TestFixture]
	public class CNYSignConverterTest
	{
		[Test]
		public void ConvertTest()
		{
			decimal source = 100001000;
			Console.WriteLine(new CNYSignConverter().ConverTo(source));
			System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN");
			Console.WriteLine(source.ToString());
		}
	}
}
