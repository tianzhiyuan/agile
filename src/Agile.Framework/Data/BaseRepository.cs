using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using Agile.Common;
using Agile.Common.Data;
using Agile.Common.DataConfig;
using Agile.Common.Exceptions;
using Agile.Common.Utils;

namespace Agile.Framework.Data
{
    /// <summary>
    /// 基础数据访问
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        protected IDbConnection Connection { get; private set; }
        /// <summary>
        /// 表元数据
        /// </summary>
        protected Metadata Metadata;
        /// <summary>
        /// 表名
        /// </summary>
        protected string TableName { get { return Metadata.TableName; } }
        /// <summary>
        /// 以逗号分割的表字段
        /// </summary>
        protected string Columns { get { return string.Join(",", Metadata.Columns); } }
        public BaseRepository(IDbConnection connection)
        {
            Connection = connection;
            Metadata = new DbContext()[typeof(TEntity)];
        }
        /// <summary>
        /// 根据Id获取模型
        /// </summary>
        /// <param name="id">模型id</param>
        /// <returns>模型</returns>
        public virtual TEntity GetById(long id)
        {
            var record = Connection.GetById<TEntity>(id);
            return record;
        }
        /// <summary>
        /// 插入模型对象，record.Id会被赋值
        /// </summary>
        /// <param name="record">被插入的对象</param>
        public virtual void Insert(TEntity record)
        {
            if (record.CreatedAt == null)
            {
                record.CreatedAt = DateTime.Now;
            }
            if (record.LastModifiedAt == null)
            {
                record.LastModifiedAt = DateTime.Now;
            }
            Connection.Insert(record);
        }

        private const int BULK_SIZE = 50;
        /// <summary>
        /// 批量插入
        /// 注意，不会返回Id值
        /// </summary>
        /// <param name="records"></param>
        public virtual void BulkInsert(TEntity[] records)
        {
            if (CollectionUtils.IsEmpty(records))
            {
                throw new ArgumentNullException("records");
            }
            foreach (var record in records)
            {
                if (record.CreatedAt == null)
                {
                    record.CreatedAt = DateTime.Now;
                }
                if (record.LastModifiedAt == null)
                {
                    record.LastModifiedAt = DateTime.Now;
                }
                
                if (Metadata.Key.Option == DatabaseGeneratedOption.Identity)
                {

                }
                else
                {
                    Require.NotNullOrEmpty(record.Id, "Id");
                }
            }
            //每次最多 BULK_SIZE 条
            int skip = 0;
            TEntity[] bulk = records.Skip(0).Take(BULK_SIZE).ToArray();
            while (CollectionUtils.NotEmpty(bulk))
            {
                Connection.BulkInsert(bulk);
                skip += BULK_SIZE;
                bulk = records.Skip(skip).Take(BULK_SIZE).ToArray();
            }

        }
        /// <summary>
        /// 根据主键删除记录
        /// </summary>
        /// <param name="id"></param>
        public virtual void DeleteById(long id)
        {
            Connection.DeleteById<TEntity>(id);
        }
        /// <summary>
        /// 部分更新对象，必须提供主键
        /// </summary>
        /// <param name="record"></param>
        public virtual void UpdateById(TEntity record)
        {
            record.LastModifiedAt = DateTime.Now;
            var oper = record as IOperator;
            
            Connection.UpdateByIdSelective(record);
        }

        /// <summary>
        /// 根据条件更新表
        /// </summary>
        /// <param name="record">更新对象</param>
        /// <param name="condition">更新条件，支持等于操作（普通类型），IN查询（数组类型)</param>
        public int UpdateByCondition(dynamic record, object condition = null)
        {
            return DapperExtensions.UpdateSelective(Connection, record, TableName, condition, null, null);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="conditions">查询条件，支持等于操作（普通类型），IN查询（数组类型)</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetList(object conditions)
        {
            return Connection.QueryList<TEntity>(conditions);
        }

        public int Count(object conditions)
        {
            return Connection.GetCount<TEntity>(conditions);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="conditions">查询条件，支持等于操作（普通类型），IN查询（数组类型)</param>
        public void DeleteByCondition(object conditions)
        {
            Connection.Delete(conditions, Metadata.TableName);
        }
    }
}
