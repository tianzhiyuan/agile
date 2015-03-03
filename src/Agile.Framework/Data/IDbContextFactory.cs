using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Data
{
	public interface IDbContextFactory
	{
		DbContext Create(Type entityType);
	}
}
