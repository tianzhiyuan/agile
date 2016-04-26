using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using Agile.Common.DataConfig;
using Agile.Common.Utils;
using Dapper;

namespace Agile.Common
{
    public static class DapperExtensions
    {
        //The default concurrency multiplier is 4
        private static readonly int _defaultLevel = 4 * Environment.ProcessorCount;

        /// <summary>
        /// 对象缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _paramCache =
            new ConcurrentDictionary<Type, List<PropertyInfo>>(_defaultLevel, 100);

        /// <summary>
        /// 查询对象缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _queryParamCache =
            new ConcurrentDictionary<Type, List<PropertyInfo>>(_defaultLevel, 100);

        private const string WhereIdEqualsId = " WHERE Id=@Id ";

        internal static string GetTableName(this Type type)
        {
            var metadata = GetMetadata(type);
            return metadata.TableName;
        }
        internal static string GetColumns(this Type type)
        {
            var metadata = GetMetadata(type);
            return string.Join(",", metadata.Columns);
        }


        /// <summary>Insert data into table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="table"></param>
        /// <param name="option"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static long Insert(this IDbConnection connection, dynamic data, string table, DatabaseGeneratedOption option, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = data as object;
            var properties = GetProperties(obj);
            var columns = string.Join(",", properties);
            var values = string.Join(",", properties.Select(p => "@" + p));
            var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES ({2})", table, columns, values);
            
            return connection.ExecuteScalar<long>(sql, obj, transaction, commandTimeout);
        }
        /// <summary>
        /// Insert data into table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static long Insert(this IDbConnection connection, object data, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            var metadata = GetMetadata(data.GetType());
            return Insert(connection, data, metadata.TableName, metadata.Key.Option, transaction, commandTimeout);
        }

        private const string NEXT_VALUE = "NEXT VALUE FOR Ids";
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection"></param>
        /// <param name="records">实体列表</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>影响行数</returns>
        public static int BulkInsert<T>(this IDbConnection connection, T[] records, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            if (records == null)
            {
                throw new ArgumentNullException("records");
            }
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            var type = typeof(T);
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("INSERT INTO [{0}]({1}) VALUES ",
                type.GetTableName(),
                type.GetColumns());
            var parameters = new DynamicParameters();
            var meta = GetMetadata(type);
            var useIdentity = meta.Key.Option == DatabaseGeneratedOption.Identity;
            var list = new List<string>();

            for (var index = 0; index < records.Length; index++)
            {
                T record = records[index];
                var paramList = new List<string>();
                if (useIdentity)
                {
                    paramList.Add(NEXT_VALUE);
                }
                else
                {
                    paramList.Add("@Id_" + index);
                }
                paramList.AddRange(
                    meta.Properties.Where(o => o.Name != "Id").Select(o => string.Format("@{0}_{1}", o.Name, index)));
                list.Add(string.Format("({0})", string.Join(",", paramList)));
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                foreach (var property in meta.Properties)
                {
                    expandoObject.Add(property.Name + "_" + index, property.PropertyInfo.GetValue(record, null));
                }
                parameters.AddDynamicParams(expandoObject);
            }
            builder.Append(string.Join(",", list));
            return connection.Execute(builder.ToString(), parameters, transaction, commandTimeout);
        }

        /// <summary>Updata data for table with a specified condition.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Update(this IDbConnection connection, dynamic data, dynamic condition, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = data as object;
            var conditionObj = condition as object;

            var wherePropertyInfos = GetPropertyInfos(conditionObj, true);

            var whereFields = string.Empty;

            if (wherePropertyInfos.Any())
            {

            }
            var objectValues = GetObjectValues(obj);
            var whereValues = GetObjectValues(conditionObj, true, true);
            if (whereValues.Any())
            {
                whereFields = " WHERE " +
                              string.Join(" AND ",
                                  whereValues.Select(
                                      p =>
                                      {
                                          var prop = wherePropertyInfos.FirstOrDefault(o => o.Name == p.Key);
                                          var str = string.Format(
                                              prop.PropertyType.IsArray ? "{0} IN @w_{0}" : "{0}=@w_{0}", p.Key);
                                          return str;
                                      }));
            }

            var updateFields = string.Join(",", objectValues.Keys.Select(k => k + "=@" + k));
            var sql = string.Format("UPDATE [{0}] SET {1}{2}", table, updateFields, whereFields);
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(objectValues);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            whereValues.ForEach(p => expandoObject.Add("w_" + p.Key, p.Value));
            parameters.AddDynamicParams(expandoObject);

            return connection.Execute(sql, parameters, transaction, commandTimeout);
        }

        /// <summary>
        /// 部分更新，不更新为null的字段
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int UpdateSelective(this IDbConnection connection, object data, string tableName, dynamic condition = null, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            var obj = data;
            var conditionObj = condition as object;

            var wherePropertyInfos = GetPropertyInfos(conditionObj, true);

            var objectValues = GetObjectValues(obj, true);
            var updateFields = string.Join(",", objectValues.Select(p => p.Key + " = @" + p.Key));
            var whereFields = string.Empty;

            var whereValues = GetObjectValues(conditionObj, true, true);
            if (whereValues.Any())
            {
                whereFields = " WHERE " +
                              string.Join(" AND ",
                                  whereValues.Select(
                                      p =>
                                      {
                                          var prop = wherePropertyInfos.FirstOrDefault(o => o.Name == p.Key);
                                          var str = string.Format(
                                              prop.PropertyType.IsArray ? "{0} IN @w_{0}" : "{0}=@w_{0}", p.Key);
                                          return str;
                                      }));
            }

            var sql = string.Format("UPDATE [{0}] SET {1}{2}", tableName, updateFields, whereFields);

            var parameters = new DynamicParameters();

            parameters.AddDynamicParams(objectValues);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            whereValues.ForEach(p => expandoObject.Add("w_" + p.Key, p.Value));
            parameters.AddDynamicParams(expandoObject);

            return connection.Execute(sql, parameters, transaction, commandTimeout);
        }

        public static int UpdateById<TEntity>(this IDbConnection connection, TEntity entity,
            IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : BaseEntity
        {
            if (entity == null || entity.Id == null)
            {
                throw new ArgumentNullException("entity");
            }
            var tableName = GetTableName(entity.GetType());

            var updatePropertyInfos = GetPropertyInfos(entity);

            var updateProperties = updatePropertyInfos.Where(p => p.Name != "Id").Select(p => p.Name);


            var updateFields = string.Join(",", updateProperties.Select(p => p + " = @" + p));


            var sql = string.Format("UPDATE [{0}] SET {1}{2}", tableName, updateFields, WhereIdEqualsId);

            return connection.Execute(sql, entity, transaction, commandTimeout);
        }

        public static int UpdateByIdSelective<TEntity>(this IDbConnection connection, TEntity entity,
            IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : BaseEntity
        {
            if (entity == null || entity.Id == null)
            {
                throw new ArgumentNullException("entity");
            }
            var tableName = GetTableName(entity.GetType());

            var updatePropertyInfos = GetPropertyInfos(entity);

            var updateProperties = updatePropertyInfos.Where(p => p.Name != "Id" && p.GetValue(entity, null) != null).Select(p => p.Name);


            var updateFields = string.Join(",", updateProperties.Where(p => p != "Id").Select(p => p + " = @" + p));


            var sql = string.Format("UPDATE [{0}] SET {1}{2}", tableName, updateFields, WhereIdEqualsId);

            return connection.ExecuteScalar<int>(sql, entity, transaction, commandTimeout);
        }
        /// <summary>Delete data from table with a specified condition.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Delete(this IDbConnection connection, dynamic condition, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var conditionObj = condition as object;
            var whereFields = string.Empty;
            var whereProperties = GetPropertyInfos(conditionObj, true);
            if (whereProperties.Any())
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(
                    p => string.Format(p.PropertyType.IsArray ? "{0} IN @{0}" : "{0}=@{0}", p.Name)));
            }

            var sql = string.Format("DELETE FROM [{0}]{1}", table, whereFields);

            return connection.Execute(sql, conditionObj, transaction, commandTimeout);
        }


        public static int DeleteById<TEntity>(this IDbConnection connection, long id, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("id");
            }
            var tableName = GetTableName(typeof(TEntity));
            var sql = string.Format("DELETE FROM [{0}]{1}", tableName, WhereIdEqualsId);
            return connection.Execute(sql, new { Id = id }, transaction, commandTimeout);
        }
        /// <summary>Get data count from table with a specified condition.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="isOr"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int GetCount(this IDbConnection connection, object condition, string table, bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryList<int>(connection, condition, table, "COUNT(1)", isOr, transaction, commandTimeout).Single();
        }

        public static int GetCount<T>(this IDbConnection connection, object condition, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            var tableName = GetTableName(typeof(T));
            return
                QueryList<int>(connection, condition, tableName, "COUNT(1)", false, transaction, commandTimeout)
                    .Single();
        }


        public static TEntity GetById<TEntity>(this IDbConnection connection, long id)
        {
            return QueryList<TEntity>(connection, new { Id = id }).FirstOrDefault();
        }
        /// <summary>Query a list of data from table with a specified condition.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <param name="isOr"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> QueryList(this IDbConnection connection, dynamic condition, string table, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryList<dynamic>(connection, condition, table, columns, isOr, transaction, commandTimeout);
        }

        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, object condition)
        {
            var type = typeof(T);
            return QueryList<T>(connection, condition, GetTableName(type), GetColumns(type), false, null, null);
        }

        /// <summary>Query a list of data from table with specified condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <param name="isOr"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, object condition, string table, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return connection.Query<T>(BuildQuerySQL(condition, table, columns, isOr), condition, transaction, true, commandTimeout);
        }
        

        private static string BuildQuerySQL(dynamic condition, string table, string selectPart = "*", bool isOr = false)
        {
            var conditionObj = condition as object;
            var properties = GetPropertyInfos(conditionObj, true);
            if (properties.Count == 0)
            {
                return string.Format("SELECT {1} FROM [{0}]", table, selectPart);
            }

            var separator = isOr ? " OR " : " AND ";
            var wherePart = string.Join(separator,
                                  properties.Select(
                                      p => (p.PropertyType.IsArray) ? p.Name + " IN @" + p.Name : p.Name + " = @" + p.Name));

            return string.Format("SELECT {2} FROM [{0}] WHERE {1}", table, wherePart, selectPart);
        }
        private static List<string> GetProperties(object obj)
        {
            if (obj == null)
            {
                return new List<string>();
            }
            var parameters = obj as DynamicParameters;
            if (parameters != null)
            {
                return parameters.ParameterNames.ToList();
            }
            return GetPropertyInfos(obj).Select(x => x.Name).ToList();
        }

        /// <summary>
        /// 获取对象包含的属性列表
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="isParam">是否为查询对象</param>
        /// <returns></returns>
        private static List<PropertyInfo> GetPropertyInfos(object obj, bool isParam = false)
        {
            if (obj == null)
            {
                return new List<PropertyInfo>();
            }
            var type = obj.GetType();
            var meta = GetMetadata(type);
            if (meta == null)
            {
                List<PropertyInfo> properties;


                if (isParam)
                {
                    if (_queryParamCache.TryGetValue(type, out properties)) return properties.ToList();
                    properties =
                        obj.GetType()
                            .GetProperties().ToList();
                    properties = properties.Where(o => QueryPropertySupported(o.PropertyType)).ToList();
                    _queryParamCache[obj.GetType()] = properties;
                }
                else
                {
                    if (_paramCache.TryGetValue(type, out properties)) return properties.ToList();
                    properties =
                        obj.GetType()
                            .GetProperties().ToList();
                    properties = properties.Where(o => _databaseTypes.Contains(o.PropertyType)).ToList();
                    _paramCache[obj.GetType()] = properties;
                }

                return properties;
            }
            return meta.Properties.Select(o => o.PropertyInfo).ToList();

        }

        private static Metadata GetMetadata(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            var context = new DbContext();
            return context[type];
        }

        /// <summary>
        /// 获取对象的值，以键值对形式返回，Key:属性名称，Value:属性值
        /// 如果对象为空，返回空字典
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="ignoreNullValues">是否忽略空值</param>
        /// <param name="isQueryParam">是否为查询对象</param>
        /// <returns></returns>
        private static IDictionary<string, object> GetObjectValues(object obj, bool ignoreNullValues = false, bool isQueryParam = false)
        {
            var dic = new Dictionary<string, object>();
            if (obj == null)
            {
                return dic;
            }
            var propertyInfos = GetPropertyInfos(obj, isQueryParam);
            foreach (var property in propertyInfos)
            {
                var value = property.GetValue(obj, null);
                if (ignoreNullValues && value == null)
                {
                    //ignore
                }
                else
                {
                    dic.Add(property.Name, value);
                }
            }
            return dic;
        }
        private static readonly Type[] _databaseTypes = new[]
        {
            typeof (int), typeof (long), typeof (byte), typeof (bool), typeof (short), typeof (string),typeof(decimal),
            typeof (int?), typeof (long?), typeof (byte?), typeof (bool?), typeof (short?),typeof(decimal?),
            typeof (DateTime),
            typeof (DateTime?)
        };

        private static bool QueryPropertySupported(Type type)
        {
            return _databaseTypes.Contains(type) || (type.IsArray && _databaseTypes.Contains(type.GetElementType()));
        }
    }
}
