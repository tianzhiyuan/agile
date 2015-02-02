using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Data
{
	[Serializable]
	public abstract class BaseEntityQuery
	{
		protected BaseEntityQuery()
		{
			
		}
		public int? Id { get; set; }
		public int[] IdList { get; set; }

		public int[] CreatorIdList { get; set; }

		public int[] LastModifierIdList { get; set; }

		public Range<DateTime> CreatedAtRange { get; set; }
		public Range<DateTime> LastModifiedAtRange { get; set; }

		public string[] Includes { get; set; }

		public int? Skip { get; set; }

		public int? Take { get; set; }

		public int? CountOfResultSet { get; set; }
		public string OrderField { get; set; }
		public OrderDirection? OrderDirection { get; set; }
		public QueryMode? Mode { get; set; }
		public bool? IsNoTracking { get; set; }
		public abstract Type ModelType { get; }

	}
	[Serializable]
	public class BaseEntityQuery<TModel> : BaseEntityQuery where TModel: BaseEntity
	{
		public override Type ModelType
		{
			get { return typeof(TModel); }
		}

		public virtual IQueryable<TModel> DoQuery(IQueryable<TModel> source)
		{
			if (Id != null)
			{
				source = source.Where(o => o.Id == Id);
			}
			if (IdList != null)
			{
				source = source.Where(o => IdList.Contains(o.Id.Value));
			}
			if (CreatorIdList != null)
			{
				source = source.Where(o => CreatorIdList.Contains(o.CreatorId.Value));
			}
			
			return source;
		}
	}
}
