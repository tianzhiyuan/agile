using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
    /// <summary>
    /// paged query interface
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TQuery"></typeparam>
    public interface IPagingQuery<TEntity, TQuery> : IRepository<TEntity> where TEntity : BaseEntity
        where TQuery : BaseQuery<TEntity>
    {
        /// <summary>
        /// 根据查询对象查询列表数据
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns>查询结果</returns>
        QueryResult<TEntity> Query(TQuery query);
    }
}
