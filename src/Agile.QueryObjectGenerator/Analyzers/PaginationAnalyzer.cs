using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.QueryObjectGenerator.Analyzers
{
	public class PaginationAnalyzer
	{
		public static readonly string KeyPropertyName = "Id";
		public string ParseStatement( PropertyInfo[] modelProperties, string sourceParamName)
		{
			var builder = new StringBuilder();
			builder.AppendLineFormat("switch(OrderField)");
			builder.AppendLine("{");
			foreach (var modelProperty in modelProperties)
			{
				if (modelProperty.PropertyType.IsValueType || modelProperty.PropertyType == typeof (string))
				{
					builder.AppendLineFormat("case \"{0}\":", modelProperty.Name);
					builder.AppendLineFormat(
						"{0}=(OrderDirection==Agile.Common.Data.OrderDirection.ASC)?{0}.OrderBy(o=>o.{1}):{0}.OrderByDescending(o=>o.{1});", sourceParamName,
						modelProperty.Name);
					builder.AppendLine("break;");
				}
				
			}
			builder.AppendLine("default:");
			builder.AppendLineFormat(
						"{0}=(OrderDirection==Agile.Common.Data.OrderDirection.ASC)?{0}.OrderBy(o=>o.{1}):{0}.OrderByDescending(o=>o.{1});", sourceParamName,
						KeyPropertyName);
			builder.AppendLine("break;");
			builder.AppendLine("}");


			return builder.ToString();
		}
	}
}
