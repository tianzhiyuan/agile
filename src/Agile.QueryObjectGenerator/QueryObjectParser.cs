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
		public static readonly string SourceParamName = "source";
		static QueryObjectParser()
		{
			Analyzers = new IAnalyzer[]
				{
					new EqualAnalyzer(), 
					new ListContainsAnalyzer(), 
					new RangeAnalyzer(), 
					new StringContainsAnalyzer(), 
					new NavigationAnalyzer(), 
				};
		}
		public static IAnalyzer[] Analyzers { get; set; }

		private readonly IList<IList<string>> queues = new List<IList<string>>();
		public QueryObjectParser()
 		{
 			for (var index = 0; index < Analyzers.Length; index++)
 			{
 				queues.Add(new List<string>());
 			}
 		}
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
			builder.AppendLine("#region override");
			builder.AppendLineFormat("public override IQueryable<{0}> DoQuery(IQueryable<{0}> {1})", modelType.Name, SourceParamName);
			builder.AppendLine("{");
			foreach (var queryProperty in queryProperties)
			{
				var context = new AnalyzeContext()
					{
						QueryProperty = queryProperty,
						ModelType = modelType,
						SourceParamName = SourceParamName,
						QueryParamName = "this"
					};
				for (var index = 0; index < Analyzers.Length; index++)
				{
					var analyzer = Analyzers[index];
					var modelProperty = analyzer.FindAttachedModelProperty(queryProperty, modelProperties);
					if (modelProperty == null) continue;
					context.ModelProperty = modelProperty;
					var statment = analyzer.ParseStatement(context);
					queues[index].Add(statment);
				}
			}
			for (var index = 0; index < Analyzers.Length; index++)
			{
				builder.Append(string.Join("", queues[index]));
			}
			builder.Append(new PaginationAnalyzer().ParseStatement(modelProperties, SourceParamName));
			builder.AppendLineFormat("return {0};", SourceParamName);
			builder.AppendLine("}");
			builder.AppendLine("#endregion");
			return builder.ToString();
		}
	}

	public class AnalyzeContext
	{
		public AnalyzeContext()
		{
			SourceParamName = "source";
			Depth = 0;
			Navigations = new string[0];
		}
		public PropertyInfo QueryProperty { get; set; }
		public PropertyInfo ModelProperty { get; set; }
		public string SourceParamName { get; set; }
		public Type ModelType { get; set; }
		public int Depth { get; set; }
		public string[] Navigations { get; set; }
		public string QueryParamName { get; set; }
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
