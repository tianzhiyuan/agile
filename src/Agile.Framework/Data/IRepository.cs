using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
	public interface IRepository<TEntity, TEntityQuery> 
		where TEntity : BaseEntity
		where TEntityQuery : BaseQuery<TEntity>
	{
		TEntity Find(int id);
		IEnumerable<TEntity> Find(int[] idList);
		IEnumerable<TEntity> FindMany(TEntityQuery filters);
		void Insert(TEntity entity);
		void Update(TEntity entity);
		void Delete(int key);
	}
	
}
