using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Components;
using Agile.Common.Utils;

namespace Agile.Common.DataConfig
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    [Component]
    public class DbContext : IAssemblyInitializer
    {
        private static readonly IDictionary<Type, Metadata> _mapping = new Dictionary<Type, Metadata>();

        public Metadata this[Type type]
        {
            get
            {
                if (_mapping.ContainsKey(type))
                {
                    return _mapping[type];
                }
                return null;
            }
        }

        public IEnumerable<Type> Keys
        {
            get { return _mapping.Keys; }
        }

        public void Initialize(params Assembly[] assemblies)
        {
            var configTypes = TypeFinder.SetScope(assemblies)
                .Where(
                    t =>
                        t.BaseType != null && !t.IsAbstract && t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == typeof (EntityConfigration<>))
                .ToArray();
            for (var index = 0; index < configTypes.Length; index++)
            {
                var configType = configTypes[index];
                var entityType = configType.BaseType.GetGenericArguments()[0];
                var config = Activator.CreateInstance(configType);
                var buildMethod = configType.GetMethod("Build");
                var metadata = buildMethod.Invoke(config, null);
                _mapping[entityType] = (Metadata) metadata;
            }
        }

        private static readonly DbContext _db = new DbContext();

        public static string TableName<TModel>()
        {
            var meta = _db[typeof (TModel)];
            if (meta == null)
            {
                throw new System.Exception("类型错误，找不到元数据" + typeof (TModel));
            }
            return meta.TableName;
        }

        public static string Columns<TModel>()
        {
            var meta = _db[typeof (TModel)];
            if (meta == null)
            {
                throw new System.Exception("类型错误，找不到元数据" + typeof (TModel));
            }
            return string.Join(",", meta.Columns);
        }
    }
}
