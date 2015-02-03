using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Agile.Framework.Install
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public class CreateTableIfNotExistsMsSql<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
	{
		private string[] _customCommands;
		public CreateTableIfNotExistsMsSql(string[] customCommands)
		{
			_customCommands = customCommands;
		}
		public void InitializeDatabase(TContext context)
		{
			bool dbExists;
			using (new TransactionScope(TransactionScopeOption.Suppress))
			{
				dbExists = context.Database.Exists();
			}
			if (dbExists)
			{
				bool createTables = false;
				//check whether tables are already created
				int numberOfTables = 0;
				foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'"))
					numberOfTables = t1;

				createTables = numberOfTables == 0;
				if (createTables)
				{
					//create all tables
					var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
					context.Database.ExecuteSqlCommand(dbCreationScript);
					if (_customCommands != null && _customCommands.Any())
					{
						foreach (var command in _customCommands)
						{
							context.Database.ExecuteSqlCommand(command);
						}
					}
					//Seed(context);
					context.SaveChanges();

				}
			}
		}
	}
}
