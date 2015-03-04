using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Data
{
	public class DefaultDbContextFactory : IDbContextFactory
	{
		public DbContext Create(Type entityType)
		{
			return new DefaultDbContext();
		}
	}
}
