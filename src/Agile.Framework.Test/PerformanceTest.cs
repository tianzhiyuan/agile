using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Framework.Data;
using NUnit.Framework;

namespace Agile.Framework.Test
{
	[TestFixture]
    public class PerformanceTest
    {
		[Test]
		public void Test()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			var times = 10000;
			DataService svc1 = new DataService()
				{
					ContextTypeString = ""
				};
			
			for (var index = 0; index < times; index++)
			{
				
			}
			stopWatch.Stop();
		}
    }
}
