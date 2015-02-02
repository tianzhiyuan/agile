using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.QueryObjectGenerator
{
	public class Util
	{
		public static bool IsSqlFieldType(Type type)
		{
			return type.IsValueType || type == typeof (string);
		}
	}
}
