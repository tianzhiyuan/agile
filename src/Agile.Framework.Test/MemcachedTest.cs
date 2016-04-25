using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Framework.Caching;
using NUnit.Framework;

namespace Agile.Framework.Test
{
	[TestFixture]
	public class MemcachedTest
	{
		

		[Test]
		public void Test1()
		{
			System.IO.Directory.CreateDirectory(@"d:\1\2\3");
			Console.WriteLine(string.Format("{0,3}", Foo.Fie));
			Console.WriteLine(string.Format("{0,3}", Foo.Field1));
			Console.WriteLine(string.Format("{0,3}", Foo.FF));
		}
	}
	public enum Foo
	{
		Field1,
		Fie,
		FF
	}
}
