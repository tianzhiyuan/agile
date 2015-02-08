using System;
using System.Collections.Generic;
using System.Linq;
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
			return type.IsValueType || type == typeof (string);
		}
		
	}
}
