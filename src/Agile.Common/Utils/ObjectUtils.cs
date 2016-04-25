using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;

namespace Agile.Common.Utils
{
    public static class ObjectUtils
    {
        private static readonly object _lock = new object();
        private static readonly IDictionary<Type, Delegate> Cloners = new Dictionary<Type, Delegate>();

        private static Delegate CreateCloner<TModel>()
        {
            var modelType = typeof(TModel);
            var fromArg = Expression.Parameter(modelType, "from");
            var toArg = Expression.Parameter(modelType, "to");
            var properties =
                modelType.GetProperties()
                         .Where(
                             o =>
                             o.CanRead && o.CanWrite &&
                             (o.PropertyType.IsValueType || TypeUtils.Is(o.PropertyType, typeof (string))))
                         .ToArray();

            var statements = properties.Select(property => Expression.Assign(Expression.Property(toArg, property), Expression.Property(fromArg, property))).Cast<Expression>().ToList();

            Delegate cloner = Expression.Lambda<Action<TModel, TModel>>(Expression.Block(statements), fromArg, toArg).Compile();

            return cloner;
        }
        /// <summary>
        /// Create an object from the source object, assign the properties respectively.
        /// Note: only assign struct or string type properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(T source) where T : class, new()
        {
            if (source == null) return null;

            var clone = new T();
            var type = typeof(T);
            Delegate cloner;
            if (!Cloners.TryGetValue(type, out cloner))
            {
                lock (_lock)
                {
                    if (!Cloners.TryGetValue(type, out cloner))
                    {
                        cloner = CreateCloner<T>();
                        Cloners.Add(type, cloner);
                    }
                }
            }

            var action = cloner as Action<T, T>;
            if (action != null) action.Invoke(source, clone);

            return clone;
        }

        private static readonly object _copyLock = new object();
        public static TDestnation CopyTo<TDestnation, TSource>(TSource source) where TDestnation : class, new()
        {
            EnsureMapperExists(typeof(TDestnation), typeof(TSource));
            return AutoMapper.Mapper.Map<TSource, TDestnation>(source);
        }

        public static TDestnation CopyTo<TDestnation>(object source) where TDestnation : class, new()
        {
            if (source == null) return null;
            EnsureMapperExists(typeof(TDestnation), source.GetType());
            return AutoMapper.Mapper.Map<TDestnation>(source);
        }

        public static object CopyTo(Type destnationType, object source)
        {
            if (source == null) return null;
            EnsureMapperExists(destnationType, source.GetType());
            return AutoMapper.Mapper.Map(destnationType, source);
        }
        private static void EnsureMapperExists(Type destType, Type sourceType)
        {
            var mapConfig = AutoMapper.Mapper.FindTypeMapFor(sourceType, destType);
            if (mapConfig == null)
            {
                lock (_lock)
                {
                    mapConfig = AutoMapper.Mapper.FindTypeMapFor(sourceType, destType);
                    if (mapConfig == null)
                    {
                        
                        AutoMapper.Mapper.CreateMap(sourceType, destType);
                    }
                }
            }
        }

        public static IDictionary<string, object> ToMap<T>(this T src) where T : class
        {
            if (src == null)
            {
                return null;
            }
            var properties = typeof(T).GetProperties();

            return properties.ToDictionary(o => o.Name, o => o.GetValue(src, null));
        }
        public static IEnumerable<IDictionary<string, object>> ToMapList<T>(IEnumerable<T> src) where T : class
        {
            if (src == null)
            {
                return null;
            }
            return src.Select(ToMap).ToArray();
        }

        public static string GetFriendlyName(this Type type)
        {
            if (type == typeof(int))
                return "int";
            if (type == typeof(short))
                return "short";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(long))
                return "long";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(string))
                return "string";
            if (type.IsGenericType)
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName).ToArray()) + ">";
            return type.Name;
        }
    }
}
