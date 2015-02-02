using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.QueryObjectGenerator.Analyzers
{
	public class RangeAnalyzer : IAnalyzer
	{
		public static readonly string Suffix = "Range";
		public string ParseStatement(AnalyzeContext context)
		{
			var queryPropertyName = context.QueryProperty.Name;
			var modelPropertyName = context.ModelProperty.Name;
			var navigation = new List<string>(context.Navigations) { modelPropertyName };
			var builder = new StringBuilder();
			builder.AppendLineFormat("if({0}.{1} != null)",context.QueryParamName, queryPropertyName)
			       .AppendLine("{")
			       .AppendLineFormat("if({0}.{1}.Left != null)", context.QueryParamName, queryPropertyName)
			       .AppendLine("{")
			       .AppendLineFormat("{0}={1}.LeftOpen?{0}.Where(o=>o.{2}>{1}.Left):{0}.Where(o=>o.{2}>={1}.Left);", context.SourceParamName, context.QueryParamName+"."+queryPropertyName, string.Join(".", navigation))
			       .AppendLine("}")
				   .AppendLineFormat("if({0}.Right != null)", context.QueryParamName + "." + queryPropertyName)
			       .AppendLine("{")
				   .AppendLineFormat("{0}={1}.RightOpen?{0}.Where(o=>o.{2}<{1}.Right):{0}.Where(o=>o.{2}<={1}.Right);", context.SourceParamName, context.QueryParamName + "." + queryPropertyName, string.Join(".", navigation))
			       .AppendLine("}")
			       .AppendLine("}");
			return builder.ToString();
		}

		public PropertyInfo FindAttachedModelProperty(PropertyInfo queryProperty, PropertyInfo[] modelProperties)
		{
			var queryPropertyName = queryProperty.Name;
			if (!queryPropertyName.EndsWith(Suffix)) return null;
			if (queryProperty.PropertyType.GetGenericTypeDefinition() != typeof (Range<>)) return null;
			return
				modelProperties.FirstOrDefault(modelProperty => queryPropertyName == modelProperty.Name + Suffix);
		}
	}
}
