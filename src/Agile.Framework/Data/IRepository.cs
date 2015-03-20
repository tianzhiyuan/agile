using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
	public interface IRepository<TEntity> where TEntity : class
	{
		TEntity Find(object id);
		bool Insert(TEntity entity);
		bool Update(TEntity entity);
		bool Delete(object key);
	}
	public interface IRepository<TEntity, TEntityQuery> : IRepository<TEntity>
		where TEntity : BaseEntity
		where TEntityQuery : BaseEntityQuery<TEntity>
	{
		IEnumerable<TEntity> FindMany(TEntityQuery filters);
	}
}
