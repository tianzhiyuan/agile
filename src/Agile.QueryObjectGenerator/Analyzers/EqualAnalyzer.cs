using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.QueryObjectGenerator.Analyzers
{
	public class EqualAnalyzer : IAnalyzer
	{
		public string ParseStatement(AnalyzeContext context)
		{
			var propertyName = context.QueryProperty.Name;
			//if(Id != null)
			//{
			//	  source = source.Where(o=>o.Id == Id);
			//}
			var navigation = new List<string>(context.Navigations) {propertyName};
			var builder = new StringBuilder();
			builder.AppendLineFormat("if({0}.{1} != null)", context.QueryParamName, propertyName)
			       .AppendLine("{")
			       .AppendLineFormat("{0} = {0}.Where(o=>o.{1} == {2}.{3});", context.SourceParamName,
			                         string.Join(".", navigation),
			                         context.QueryParamName, propertyName)
			       .AppendLine("}");
			return builder.ToString();
		}

		public PropertyInfo FindAttachedModelProperty(PropertyInfo queryProperty, PropertyInfo[] modelProperties)
		{
			var queryPropertyName = queryProperty.Name;
			foreach (var modelProperty in modelProperties)
			{
				if (modelProperty.PropertyType.IsValueType || modelProperty.PropertyType == typeof (string))
				{
					if (modelProperty.Name == queryPropertyName) return modelProperty;
				}
			}
			return null;
		}

		
	}
}
