using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Data
{
	public interface IPagingOption<TEntity> where TEntity : class
	{
		IQueryable<TEntity> Apply(IQueryable<TEntity> source);
	}
}
