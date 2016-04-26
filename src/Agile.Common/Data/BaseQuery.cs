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
	public abstract class BaseQuery
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
        /// 创建时间范围
        /// </summary>
        public DateTime? CreatedAtFrom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreatedAtTo { get; set; }
        /// <summary>
        /// 更新时间范围
        /// </summary>
        public DateTime? LastModifiedAtFrom { get; set; }
        /// <summary>
        /// 更新时间范围
        /// </summary>
        public DateTime? LastModifiedAtTo { get; set; }

        /// <summary>
        /// Id范围
        /// </summary>
        public long? IdFrom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? IdTo { get; set; }
        /// <summary>
        /// orderby field. default "Id" field
        /// </summary>
        public string OrderBy { get; set; }
		/// <summary>
		/// order direction. i.e. OrderBy/OrderByDescending . default <see cref="Data.OrderDirection.DESC"/>
		/// </summary>
		public OrderDirection? OrderDirection { get; set; }

        /// <summary>
        /// 方向列表
        /// </summary>
        public KeyValuePair<string, OrderDirection>[] DirectionList { get; set; } 
		/// <summary>
		/// corresponding entity type 
		/// </summary>
		public abstract Type ModelType { get; }

		#endregion
	}
	[Serializable]
	public class BaseQuery<TModel> : BaseQuery where TModel: BaseEntity
	{
		public override Type ModelType { get; } = typeof(TModel);
        
	}
}
