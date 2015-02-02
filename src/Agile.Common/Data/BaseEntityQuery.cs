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
		public int? Id { get; set; }
		public int[] IdList { get; set; }
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
			
			return source;
		}
	}
}
