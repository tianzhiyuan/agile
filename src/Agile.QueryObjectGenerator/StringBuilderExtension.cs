using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.QueryObjectGenerator
{
	public static class StringBuilderExtension
	{
		public static StringBuilder AppendLine(this StringBuilder builder, string value)
		{
			return builder.Append(value).AppendLine();
		}

		public static StringBuilder AppendLineFormat(this StringBuilder builder, string format, params object[] parameters)
		{
			return builder.AppendFormat(format, parameters).AppendLine();
		}
	}
}
