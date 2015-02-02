using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Utilities;

namespace Agile.QueryObjectGenerator.Analyzers
{
	public class ListContainsAnalyzer : IAnalyzer
	{
		static ListContainsAnalyzer()
		{
			Suffix = "List";
		}
		public static string Suffix { get; set; }
		public string ParseStatement(AnalyzeContext context)
		{
			var queryPropertyName = context.QueryProperty.Name;
			var modelPropertyName = context.ModelProperty.Name;
			var navigation = new List<string>(context.Navigations) { modelPropertyName };
			var builder = new StringBuilder();
			var modelPropertyType = context.ModelProperty.PropertyType;

			builder.AppendLineFormat("if({0}.{1} != null)", context.QueryParamName, queryPropertyName)
			       .AppendLine("{");
			if (modelPropertyType.IsGenericType)
			{
				builder.AppendLineFormat("{0} = {0}.Where(o=>{3}.{1}.Contains(o.{2}.Value));", context.SourceParamName, queryPropertyName,
				                         string.Join(".", navigation), context.QueryParamName);
			}
			else
			{
				builder.AppendLineFormat("{0} = {0}.Where(o=>{3}.{1}.Contains(o.{2}));", context.SourceParamName, queryPropertyName,
										 string.Join(".", navigation), context.QueryParamName);
			}
			       
			builder.AppendLine("}");
			return builder.ToString();
		}

		public PropertyInfo FindAttachedModelProperty(PropertyInfo queryProperty, PropertyInfo[] modelProperties)
		{
			var queryPropertyName = queryProperty.Name;
			if (!queryPropertyName.EndsWith(Suffix)) return null;
			if (!queryProperty.PropertyType.IsArray) return null;
			var elementType = queryProperty.PropertyType.GetElementType();
			if (!elementType.IsValueType && elementType != typeof(string)) return null;
			if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof (Nullable<>))
			{
				elementType = elementType.GetGenericArguments()[0];
			}
			foreach (var modelProperty in modelProperties)
			{
				var propertyType = modelProperty.PropertyType;
				if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					propertyType = propertyType.GetGenericArguments()[0];
				}
				if (propertyType != elementType) continue;
				if (modelProperty.Name + Suffix == queryPropertyName)
				{
					return modelProperty;
				}
			}
			return null;
		}
	}
}
