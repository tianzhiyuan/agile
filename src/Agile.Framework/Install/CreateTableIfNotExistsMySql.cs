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
	public class CreateTableIfNotExistsMySql<TContext> : CreateDatabaseIfNotExists<TContext> where TContext : DbContext
	{
		private string[] _customCommands;
		public CreateTableIfNotExistsMySql(string[] customCommands)
		{
			_customCommands = customCommands;
		}
		protected override void Seed(TContext context)
		{
			base.Seed(context);
			if (_customCommands != null && _customCommands.Any())
			{
				foreach (var command in _customCommands)
				{
					context.Database.ExecuteSqlCommand(command);
				}
			}
		}
	}
}
