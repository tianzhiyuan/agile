using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
	public static class RepositoryExtensions
	{
		public static IEnumerable<TEntity> GetManyNoLock<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IPagingOption<TEntity> pagingOption = null) where TEntity : BaseEntity
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
												 new TransactionOptions()
													 {
														 IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
													 }))
			{
				scope.Complete();
				return repository.GetMany(predicate, pagingOption);
			}
		}
		public static int CountNoLock<TEntity>(this IRepository<TEntity> repository,
																Expression<Func<TEntity, bool>> predicate)
			where TEntity : BaseEntity
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
												 new TransactionOptions()
												 {
													 IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
												 }))
			{
				scope.Complete();
				return repository.Count(predicate);
			}
		}
		public static IDictionary<int, TEntity> GetManyAsDictionary<TEntity>(this IRepository<TEntity> repository, IEnumerable<int> keys) where TEntity : BaseEntity
		{
			return repository.GetMany(o => keys.Contains(o.Id)).ToDictionary(o => o.Id, o => o);
		}
	}
}
