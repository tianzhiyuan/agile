using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Data
{
	/// <summary>
	/// represents query conditions object against a entity
	/// 
	/// </summary>
	/// <remarks>
	/// NOTICE!
	/// each property of a query-object represents a certain kind of query condition against relevent property of the entity.
	/// there are 5 kind of query condition, each kind has a certain naming(and type) restriction
	/// 1. Equal : PropertyName(QueryObject) = PropertyName(EntityObject). eg. UserQuery.Id, User.Id
	/// 2. List Contains : PropertyName(QueryObject) = PropertyName(EntityObject)+"List". eg. UserQuery.IdList, User.Id
	/// 3. String Contains : PropertyName(QueryObject) = PropertyName(EntityObject)+"Pattern". eg. UserQuery.NamePattern, User.Name(Note: property type must be string)
	/// 4. Range : PropertyName(QueryObject)=PropertyName(EntityObject)+"Range". eg. UserQuery.CreatedAtRange, User.CreatedAt
	/// 5. Navigation Query: PropertyName(QueryObject)=PropertyName(EntityObject)+"Query". eg.UserQuery.RoleQuery, User.Role
	/// </remarks>
	[Serializable]
	public abstract class BaseEntityQuery
	{
		#region query conditions
		/// <summary>
		/// represents equal comparision against BaseEntity.Id
		/// </summary>
		public int? Id { get; set; }
		/// <summary>
		/// represents query if contains BaseEntity.Id
		/// </summary>
		public int[] IdList { get; set; }
		/// <summary>
		/// represents range comparision against BaseEntity.Range
		/// </summary>
		public Range<DateTime> CreatedAtRange { get; set; }
		public Range<DateTime> LastModifiedAtRange { get; set; }
		#endregion

		#region query settings
		/// <summary>
		/// eager loading of navigation property
		/// </summary>
		public string[] Includes { get; set; }
		/// <summary>
		/// skip number
		/// </summary>
		public int? Skip { get; set; }
		/// <summary>
		/// take number
		/// </summary>
		public int? Take { get; set; }
		/// <summary>
		/// out parameter. when query is executed, the total count of the result set will assign to this property.
		/// </summary>
		public int? CountOfResultSet { get; set; }
		/// <summary>
		/// orderby field. default "Id" field
		/// </summary>
		public string OrderField { get; set; }
		/// <summary>
		/// order direction. i.e. OrderBy/OrderByDescending . default <see cref="Data.OrderDirection.DESC"/>
		/// </summary>
		public OrderDirection? OrderDirection { get; set; }
		/// <summary>
		/// query mode. default<see cref="QueryMode.Both"/>
		/// </summary>
		public QueryMode? Mode { get; set; }
		/// <summary>
		/// corresponding entity type 
		/// </summary>
		public abstract Type ModelType { get; }
		/// <summary>
		/// indicates whether this query use nolock. (readuncommited)
		/// </summary>
		public bool? IsNoLock { get; set; }
		#endregion
	}
	[Serializable]
	public class BaseEntityQuery<TModel> : BaseEntityQuery where TModel: BaseEntity
	{
		public override Type ModelType
		{
			get { return typeof(TModel); }
		}
		/// <summary>
		/// use linq to entities to execute query
		/// override this.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public virtual IQueryable<TModel> Apply(IQueryable<TModel> source)
		{
			return source;
		}
	}
}
