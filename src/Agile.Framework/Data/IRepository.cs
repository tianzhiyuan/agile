using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
	public interface IRepository<TEntity> where TEntity : BaseEntity
	{
		IQueryable<TEntity> GetTable();
		TEntity Get(int id);
		IEnumerable<TEntity> GetMany(IEnumerable<int> ids);
		IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate, IPagingOption<TEntity> pagingOption = null);

		IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate, IPagingOption<TEntity> pagingOption,
		                             string[] includes); 
		int Count(Expression<Func<TEntity, bool>> predicate);

		void Update(TEntity entity);
		void Delete(TEntity entity);
		void Create(TEntity entity);

		void UpdateBatch(Expression<Func<TEntity, TEntity>> updateProperties, Expression<Func<TEntity, bool>> predicate);
		void DeleteBatch(Expression<Func<TEntity, bool>> predicate);


	}
}
