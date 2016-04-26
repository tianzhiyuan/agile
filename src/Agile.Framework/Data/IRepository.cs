using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Data
{
    /// <summary>
    /// repository interface
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// get entity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(long id);
        /// <summary>
        /// insert entity into database
        /// </summary>
        /// <param name="record"></param>
        void Insert(TEntity record);
        /// <summary>
        /// delete entity by id
        /// </summary>
        /// <param name="id"></param>
        void DeleteById(long id);
        /// <summary>
        /// partial update entity by id
        /// </summary>
        /// <param name="record"></param>
        void UpdateById(TEntity record);

        /// <summary>
        /// partial update entity by condition
        /// </summary>
        /// <param name="record"></param>
        /// <param name="condition"></param>
        int UpdateByCondition(dynamic record, object condition = null);

    }

}
