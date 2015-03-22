using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using DapperExtensions;

namespace Agile.Framework.Data
{
	/// <summary>
	/// 基础数据库访问层
	/// <remarks>使用DapperExtensions访问数据库，如果性能要求很高，请使用dapper或者原生ADO.Net或存储过程访问</remarks>
	/// </summary>
	public abstract class DapperRepository<TEntity, TEntityQuery> : IRepository<TEntity, TEntityQuery>
		where TEntity : BaseEntity
		where TEntityQuery : BaseEntityQuery<TEntity>
	{
		public const string DefaultConnectionName = "DefaultConnection";
		protected IDbConnectionFactory _connectionFactory;
		protected string _connectionName;
		protected DapperRepository(IDbConnectionFactory factory, string connectionName)
		{
			_connectionFactory = factory;
			_connectionName = string.IsNullOrWhiteSpace(connectionName) ? DefaultConnectionName : connectionName;
		}
		#region IRepository Members
		public virtual void Insert(TEntity entity)
		{
			using (var conn = GetConnection())
			{
				conn.Open();
				conn.Insert<TEntity>(entity);
			}
		}

		public virtual void Update(TEntity entity)
		{
			using (var conn = GetConnection())
			{
				conn.Open();
				conn.Update<TEntity>(entity);
			}
		}

		public virtual void Delete(int key)
		{
			using (var conn = GetConnection())
			{
				conn.Open();
				var predicate = Predicates.Field<TEntity>(t => t.Id, Operator.Eq, key);
				conn.Delete<TEntity>(predicate);
			}
		}

		public TEntity Find(int id)
		{
			using (var conn = GetConnection())
			{
				conn.Open();
				var predicate = Predicates.Field<TEntity>(t => t.Id, Operator.Eq, id);
				return conn.Get<TEntity>(predicate);
			}
		}

		public IEnumerable<TEntity> Find(int[] idList)
		{
			using (var conn = GetConnection())
			{
				conn.Open();
				var predicate = Predicates.Field<TEntity>(t => t.Id, Operator.Eq, idList);
				return conn.GetList<TEntity>(predicate);
			}
		}

		public abstract IEnumerable<TEntity> FindMany(TEntityQuery filters);
		#endregion
		protected virtual IDbConnection GetConnection()
		{
			return _connectionFactory.CreateConnection(_connectionName);
		}
		
	}
}
