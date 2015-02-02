using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using Agile.Common.Utilities;
using Agile.QueryObjectGenerator.Analyzers;

namespace Agile.QueryObjectGenerator
{
	public class QueryObjectParser
	{
		static QueryObjectParser()
		{
			Analyzers = new IAnalyzer[]
				{
					new EqualAnalyzer(), 
					new ListContainsAnalyzer(), 
					new RangeAnalyzer(), 
					new StringContainsAnalyzer(), 
				};
		}
		public static IAnalyzer[] Analyzers { get; set; }
		public string GenerateQueryStatement(Type queryType)
		{
			if (!TypeUtils.IsBaseEntityQuery(queryType))
			{
				throw new ArgumentException("not a valid query type", "queryType");
			}
			Type baseType = queryType.BaseType;
			while (!baseType.IsGenericType)
			{
				baseType = baseType.BaseType;
			}
			var modelType = baseType.GetGenericArguments()[0];
			if (!TypeUtils.IsBaseEntity(modelType))
			{
				throw new ArgumentException("generic type is not a valid model type", "queryType");
			}

			var queryProperties = queryType.GetProperties();
			var modelProperties = modelType.GetProperties();
			var builder = new StringBuilder();
			foreach (var queryProperty in queryProperties)
			{
				var context = new AnalyzeContext()
					{
						QueryProperty = queryProperty,
						ModelType = modelType
					};
				foreach (var analyzer in Analyzers)
				{
					var modelProperty = analyzer.FindAttachedModelProperty(queryProperty, modelProperties);
					if (modelProperty == null) continue;
					context.ModelProperty = modelProperty;
					var statment = analyzer.ParseStatement(context);
					builder.Append(statment);
				}
			}
			builder.Append(new PaginationAnalyzer().ParseStatement(modelProperties, "source"));
			builder.AppendLineFormat("return {0};", "source");
			return builder.ToString();
		}
	}

	public class AnalyzeContext
	{
		public AnalyzeContext()
		{
			SourceParamName = "source";
		}
		public PropertyInfo QueryProperty { get; set; }
		public PropertyInfo ModelProperty { get; set; }
		public string SourceParamName { get; set; }
		public Type ModelType { get; set; }
	}

	/// <summary>
	/// User string concat to generate code.
	/// consider using codedom
	/// </summary>
	public interface IAnalyzer
	{
		string ParseStatement(AnalyzeContext context);
		PropertyInfo FindAttachedModelProperty(PropertyInfo queryProperty, PropertyInfo[] modelProperties);
	}
	
}
