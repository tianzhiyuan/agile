using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Utils
{
    public static class EnumUtil
    {
        private static readonly IDictionary<Type, IDictionary<int, string>> _enumMap =
            new Dictionary<Type, IDictionary<int, string>>(100);

        private static readonly IDictionary<Type, IDictionary<string, string>> _literalMap =
            new Dictionary<Type, IDictionary<string, string>>(100);

        public static IDictionary<int, string> GetDescMap<TEnum>()
        {
            return GetDescMap(typeof(TEnum));
        }

        public static IDictionary<int, string> GetDescMap(Type enumType)
        {
            if (_enumMap.ContainsKey(enumType))
            {
                return _enumMap[enumType];
            }
            var dictionary = new Dictionary<int, string>();
            if (enumType.IsEnum)
            {
                foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var desc = field.GetCustomAttribute<DescriptionAttribute>();
                    var name = field.Name;
                    if (desc != null)
                    {
                        name = desc.Description;
                    }
                    var value = (int)field.GetValue(null);
                    dictionary[value] = name;
                }
            }
            _enumMap[enumType] = dictionary;
            return dictionary;

        }

        public static IDictionary<string, string> GetLiteralDescMap(Type enumType)
        {
            if (_literalMap.ContainsKey(enumType))
            {
                return _literalMap[enumType];
            }
            var dictionary = new Dictionary<string, string>();

            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var desc = field.GetCustomAttribute<DescriptionAttribute>();
                var name = field.Name;
                if (desc != null)
                {
                    name = desc.Description;
                }
                var value = (string)field.GetValue(null);
                dictionary[value] = name;
            }
            _literalMap[enumType] = dictionary;
            return dictionary;

        }


        public static string GetDesc<TEnum>(int enumValue)
        {
            var dictionary = GetDescMap(typeof(TEnum));
            if (dictionary.ContainsKey(enumValue))
            {
                return dictionary[enumValue];
            }
            return string.Empty;
        }

        public static string GetDesc<TEnum>(string enumValue)
        {
            var dictionary = GetLiteralDescMap(typeof(TEnum));
            if (string.IsNullOrWhiteSpace(enumValue))
            {
                return string.Empty;
            }
            if (dictionary.ContainsKey(enumValue))
            {
                return dictionary[enumValue];
            }
            return string.Empty;
        }
        /// <summary>  
        /// 获取枚举项的指定Attribute  
        /// </summary>  
        /// <typeparam name="T">自定义的Attribute</typeparam>  
        /// <param name="source">枚举</param>  
        /// <returns>返回枚举,否则返回null</returns>  
        public static T GetCustomAttribute<T>(Enum source) where T : Attribute
        {
            Type sourceType = source.GetType();
            string sourceName = Enum.GetName(sourceType, source);
            FieldInfo field = sourceType.GetField(sourceName);
            object[] attributes = field.GetCustomAttributes(typeof(T), false);
            return attributes.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// 获取字符串枚举的指定Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetLiteralCustomAttribute<T>(Type enumType, string value) where T : Attribute
        {
            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if ((string)field.GetValue(null) == value)
                {
                    return (T)field.GetCustomAttribute(typeof(T), false);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取枚举遍历
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, int>> GetKeyValuePairs(this Type type)
        {
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            if (type.IsEnum)
            {

                var values = Enum.GetValues(type);
                for (int i = 0; i < values.Length; i++)
                {
                    var en = Enum.Parse(type, values.GetValue(i).ToString());
                    // 获取枚举字段。
                    FieldInfo fieldInfo = type.GetField(values.GetValue(i).ToString());
                    if (fieldInfo != null)
                    {
                        // 获取描述的属性。
                        DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                            typeof(DescriptionAttribute), false) as DescriptionAttribute;
                        if (attr != null)
                        {
                            result.Add(new KeyValuePair<string, int>(attr.Description, (int)en));
                        }
                    }
                }

            }
            return result;
        }

        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <returns>枚举想的描述信息。</returns>
        public static string GetDescription(this Enum value)
        {
            Type enumType = value.GetType();
            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                        typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
