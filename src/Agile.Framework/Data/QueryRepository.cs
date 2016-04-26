using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using Agile.Common.Utils;
using Dapper;

namespace Agile.Framework.Data
{
    /// <summary>
    /// 带分页查询的数据访问
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TQuery"></typeparam>
    public abstract class QueryRepository<TEntity, TQuery> : BaseRepository<TEntity>, IPagingQuery<TEntity, TQuery>
        where TEntity : BaseEntity
        where TQuery : BaseQuery<TEntity>
    {
        protected const string COUNT = "COUNT(1)";
        protected QueryRepository(IDbConnection connection) : base(connection)
        {

        }
        /// <summary>
        /// 根据条件查询对象
        /// 注意：一般情况下子类不需要重写这个方法，如果需要查询时映射导航属性再重写
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual QueryResult<TEntity> Query(TQuery query)
        {
            var queryResult = new QueryResult<TEntity>();
            var count = 0;
            queryResult.List = Enumerable.Empty<TEntity>();
            if (query.Take == null || query.Take.Value > 0)
            {
                //query.Take == null 表示默认获取所有列表
                //get list
                var listSql = GetListSql(query);

                queryResult.List = Connection.Query<TEntity>(listSql, query);//重写方法一般只需要修改这一段代码
                count = queryResult.List.Count();
            }
            if (query.Take != null || query.Skip != null)
            {
                //如果 query.Take == null && query.Skip == null 则查询出的列表数量一定等于总数量，这种情况下不需要再次查询count
                //反之，则需要
                //query count
                var countSql = GetCountSql(query);
                count = Connection.ExecuteScalar<int>(countSql, query);
            }
            queryResult.Count = count;
            return queryResult;
        }

        /// <summary>
        /// 根据查询对象构造列表查询Sql，非单表查询请重写该方法
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual string GetListSql(TQuery query)
        {
            var builder = SqlBuilder.Begin();
            builder.Select(Columns)
                .From(TableName);
            builder.Where();
            AppendWhere(builder, query);
            var orderByField = "Id";
            switch (query.OrderBy)
            {
                default:
                    orderByField = "Id";
                    break;
            }
            builder.OrderBy(orderByField, query.OrderDirection);
            if (query.Take != null)
            {
                builder.Take(query.Take.Value);
            }
            if (query.Skip != null)
            {
                builder.Skip(query.Skip.Value);
            }
            return builder.End();
        }
        /// <summary>
        /// 根据查询对象构造数量查询，非单表查询请重写该方法
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual string GetCountSql(TQuery query)
        {
            var builder = SqlBuilder.Begin()
                .SelectCount()
                .From(TableName)
                .Where();
            AppendWhere(builder, query);
            return builder.End();
        }
        /// <summary>
        /// 拼接Where条件，始终重写该方法，注意重写方法中调用 base.AppendWhere
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="query"></param>
        protected virtual void AppendWhere(SqlBuilder builder, TQuery query)
        {
            AppendWhere(builder, query, string.Empty);
        }

        /// <summary>
        /// 拼接Where条件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="query"></param>
        /// <param name="alias">当前表别名</param>
        protected void AppendWhere(SqlBuilder builder, TQuery query, string alias)
        {
            var local = alias;
            if (!string.IsNullOrEmpty(local))
            {
                local = local + ".";
            }
            if (query.CreatedAtFrom != null)
            {
                builder.And(local + "CreatedAt>=@CreatedAtFrom");
            }
            if (query.CreatedAtTo != null)
            {
                builder.And(local + "CreatedAt<=@CreatedAtTo");
            }
            if (query.LastModifiedAtFrom != null)
            {
                builder.And(local + "LastModifiedAt>=@LastModifiedAtFrom");
            }
            if (query.LastModifiedAtTo != null)
            {
                builder.And(local + "LastModifiedAt<=@LastModifiedAtTo");
            }
            if (query.IdFrom != null)
            {
                builder.And(local + "Id>=@IdFrom");
            }
            if (query.IdTo != null)
            {
                builder.And(local + "Id<=@IdTo");
            }
            if (!CollectionUtils.IsEmpty(query.IdList))
            {
                builder.And(local + "Id IN @Ids");
            }
            if (query.Id != null)
            {
                builder.And(local + "Id=@Id");
            }
        }

        protected virtual void AppendWhere(StringBuilder builder, TQuery query)
        {
            AppendWhere(builder, query, "");
        }
        protected void AppendWhere(StringBuilder builder, TQuery query, string prefix)
        {
            var local = prefix;
            if (!string.IsNullOrEmpty(local))
            {
                local = local + ".";
            }
            if (query.CreatedAtFrom != null)
            {
                builder.Append(local + "CreatedAt>=@CreatedAtFrom");
            }
            if (query.CreatedAtTo != null)
            {
                builder.Append(local + "CreatedAt<=@CreatedAtTo");
            }
            if (query.LastModifiedAtFrom != null)
            {
                builder.Append(local + "LastModifiedAt>=@LastModifiedAtFrom");
            }
            if (query.LastModifiedAtTo != null)
            {
                builder.Append(local + "LastModifiedAt<=@LastModifiedAtTo");
            }
            if (query.IdFrom != null)
            {
                builder.Append(local + "Id>=@IdFrom");
            }
            if (query.IdTo != null)
            {
                builder.Append(local + "Id<=@IdTo");
            }
            if (!CollectionUtils.IsEmpty(query.IdList))
            {
                builder.Append(local + "Id IN @Ids");
            }
            if (query.Id != null)
            {
                builder.Append(local + "Id=@Id");
            }
        }
    }
}
