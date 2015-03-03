using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
	public class PagingOption<TEntity, TSortKey> : IPagingOption<TEntity> where TEntity : BaseEntity
	{
		public PagingOption()
		{
			this.PageSize = 0;
			this.PageNumber = 1;
			this.IsDescending = true;
		}
		/// <summary>
		/// how many items does one page contains
		/// </summary>
		public int PageSize { get; set; }
		/// <summary>
		/// 第几页 从1开始
		/// </summary>
		public int PageNumber { get; set; }
		/// <summary>
		/// skip number
		/// </summary>
		public int Skip { get { return (PageNumber - 1)*PageSize; } }
		/// <summary>
		/// take number
		/// </summary>
		public int Take { get { return PageSize; } }
		
		/// <summary>
		/// orderby field. default "Id" field
		/// </summary>
		public Expression<Func<TEntity, TSortKey>> OrderBy { get; set; }
		/// <summary>
		/// order direction. i.e. OrderBy/OrderByDescending . default <see cref="Common.Data.OrderDirection.DESC"/>
		/// </summary>
		public bool IsDescending { get; set; }
		public IQueryable<TEntity> Apply(IQueryable<TEntity> source)
		{
			if (OrderBy != null)
			{
				source = IsDescending ? source.OrderByDescending(OrderBy) : source.OrderBy(OrderBy);
			}
			if (Skip > 0 || Take > 0)
			{
				if (OrderBy == null)
				{
					source = IsDescending ? source.OrderByDescending(o => o.Id) : source.OrderBy(o => o.Id);
				}
				source = source.Skip(Skip).Take(Take);
			}
			IQueryable<TEntity> result = Take > 0 ? source : Enumerable.Empty<TEntity>().AsQueryable();
			return result;
		}
	}
	
}
