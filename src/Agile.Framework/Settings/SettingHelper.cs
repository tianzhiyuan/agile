using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Settings
{
	public class SettingHelper
	{
		/// <summary>
		/// setting中支持的类型
		/// Enum/string/常见的基础类型(int/long etc.)以及这些类型的数组
		/// 
		/// 暂时不支持数组
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool Supported(Type type)
		{
			return type.IsValueType || type == typeof(string);
		}

		public static TSetting ConstructDefault<TSetting>() where TSetting : class, ISetting, new()
		{
			var setting = new TSetting();
			foreach (var property in typeof(TSetting).GetProperties().Where(o => Supported(o.PropertyType)))
			{
				var attribute = property.GetCustomAttributes<DefaultValueAttribute>(true).FirstOrDefault();
				if (attribute != null)
				{
					property.SetValue(setting, attribute.Value);
				}
				else
				{
					
				}
			}
			return setting;
		}
		public static ISetting ConstructDefault(Type type)
		{
			var setting = Activator.CreateInstance(type);
			foreach (var property in type.GetProperties().Where(o => Supported(o.PropertyType)))
			{
				var attribute = property.GetCustomAttributes<DefaultValueAttribute>(true).FirstOrDefault();
				if (attribute != null)
				{
					property.SetValue(setting, attribute.Value);
				}
				else
				{

				}
			}
			return setting as ISetting;
		}
	}
}
