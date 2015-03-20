using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
	public abstract class BaseRepository<TEntity, TEntityQuery> : IRepository<TEntity, TEntityQuery>
		where TEntity : BaseEntity
		where TEntityQuery : BaseEntityQuery<TEntity>
	{
		protected BaseRepository()
		{
			
		} 
		public TEntity Find(object id)
		{
			throw new NotImplementedException();
		}

		public bool Insert(TEntity entity)
		{
			throw new NotImplementedException();
		}

		public bool Update(TEntity entity)
		{
			throw new NotImplementedException();
		}

		public bool Delete(object key)
		{
			throw new NotImplementedException();
		}

		public abstract IEnumerable<TEntity> FindMany(TEntityQuery filters);
	}
}
