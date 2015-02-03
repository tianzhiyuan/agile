using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Utilities;

namespace Agile.QueryObjectGenerator
{
	public class Util
	{
		public static bool IsSqlFieldType(Type type)
		{
			return type.IsValueType || type == typeof (string);
		}

		public static Type[] GetAllQueryTypes(Assembly[] assemblies)
		{
			var queryTypes = new List<Type>();
			foreach(var assembly in assemblies)
			{
				try
				{
					foreach (var type in assembly.GetTypes())
					{
						if (TypeUtils.IsBaseEntityQuery(type))
						{
							queryTypes.Add(type);
						}
					}
				}
				catch
				{
					
				}
			}
			return queryTypes.ToArray();
		}
	}
}
