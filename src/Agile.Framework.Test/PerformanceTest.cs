using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
			var times = 10000;
			DbContextType = typeof (TestDb);
			GetContext();
			new TestDb("test");
			stopWatch.Start();
			for (var index = 0; index < times; index++)
			{
				GetContext();
			}
			stopWatch.Stop();
			Console.WriteLine(stopWatch.ElapsedMilliseconds);
		}
		private Func<string, DbContext> _createContext;
		private readonly object _contextCreatorLocker = new object();
		private Type DbContextType;
		protected DbContext GetContext()
		{
			Func<string, DbContext> handler = _createContext;
			if (handler == null)
			{
				lock (_contextCreatorLocker)
				{
					if (_createContext == null)
					{
						var type = DbContextType;
						if (type == null) throw new ArgumentNullException("DbContextType");

						var constructors = type.GetConstructors();
						var arg = Expression.Parameter(typeof(string));
						var predicateMethod = typeof(string).GetMethod("IsNullOrWhiteSpace");
						handler = _createContext = Expression.Lambda<Func<string, DbContext>>(
							Expression.Condition(
								Expression.Call(predicateMethod, arg),
								Expression.New(constructors.First(o => o.GetParameters().Length == 0)),
								Expression.New(constructors.First(o => o.GetParameters().Length == 1), arg)
							),
							arg
						).Compile();
					}
					else
					{
						handler = _createContext;
					}
				}
			}

			var db = handler("test");
			db.Configuration.AutoDetectChangesEnabled = false;
			db.Configuration.ProxyCreationEnabled = false;
			return db;
		}
    }
	public class TestDb: DbContext
	{
		public TestDb()
			: base("Db")
		{
			
		}

		public TestDb(string name) : base(name)
		{
			
		}
	}
}
