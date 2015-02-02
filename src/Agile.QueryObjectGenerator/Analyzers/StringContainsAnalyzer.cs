using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.QueryObjectGenerator.Analyzers
{
	public class StringContainsAnalyzer : IAnalyzer
	{
		static StringContainsAnalyzer()
		{
			Suffix = "Pattern";
		}
		public static string Suffix { get; set; }
		public string ParseStatement(AnalyzeContext context)
		{
			var queryPropertyName = context.QueryProperty.Name;
			var modelPropertyName = context.ModelProperty.Name;
			var navigation = new List<string>(context.Navigations) { modelPropertyName };
			var builder = new StringBuilder();
			builder.AppendLineFormat("if({0} != null)", context.QueryParamName + "." + queryPropertyName)
			       .AppendLine("{")
				   .AppendLineFormat("{0}={0}.Where(o=>o.{1}.Contains({2}));", context.SourceParamName, string.Join(".", navigation), context.QueryParamName + "." + queryPropertyName)
			       .AppendLine("}");
			return builder.ToString();
		}

		public PropertyInfo FindAttachedModelProperty(PropertyInfo queryProperty, PropertyInfo[] modelProperties)
		{
			var queryPropertyName = queryProperty.Name;
			if (queryProperty.PropertyType != typeof (string)) return null;
			if (!queryPropertyName.EndsWith(Suffix)) return null;
			return
				modelProperties.Where(o => o.PropertyType == typeof (string))
				               .FirstOrDefault(modelProperty => queryPropertyName == modelProperty.Name + Suffix);
		}
	}
}
