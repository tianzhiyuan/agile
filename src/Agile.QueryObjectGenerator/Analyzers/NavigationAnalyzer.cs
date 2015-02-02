using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Utilities;

namespace Agile.QueryObjectGenerator.Analyzers
{
	public class NavigationAnalyzer : IAnalyzer
	{
		static NavigationAnalyzer()
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
		private void Init()
		{
			queues = new List<IList<string>>();
			for (var index = 0; index < Analyzers.Length; index++)
			{
				queues.Add(new List<string>());
			}
		}
		public NavigationAnalyzer()
		{
			
		}
		public static IAnalyzer[] Analyzers { get; set; }
		public static readonly string Suffix = "Query";
		public static readonly int MaxDepth = 1;
		private IList<IList<string>> queues;
		public string ParseStatement(AnalyzeContext context)
		{
			Init();
			/**
             *      ***Navigation
             *      if(concreteQuery.UserQuery != null)
             *     ｛
             *          {
             *              var naviQuery = concreteQuery.UserQuery;
             *              if(naviQuery.Id != null){
             *                  source = source.Where(t=>t.User.Id == naviQuery.Id);
             *              }
			 *              if(naviQuery.RoleQuery != null){
			 *					var naviQuery_2 = naviQuery.RoleQuery;
			 *					
			 *              }
             *          }
             *      ｝
             */
			if (context.Depth >= MaxDepth) return "";
			var queryPropertyName = context.QueryProperty.Name;
			var modelPropertyName = context.ModelProperty.Name;
			var naviQueryParam = "naviQuery_" + (context.Depth + 1);
			var builder = new StringBuilder();
			builder.AppendLineFormat("if({0} != null)", context.QueryParamName+"."+queryPropertyName);
			builder.AppendLine("{");
			builder.AppendLineFormat("var {0}={1}.{2};", naviQueryParam, context.QueryParamName, queryPropertyName);
			var naviQueryProperties = context.QueryProperty.PropertyType.GetProperties();
			var naviModelProperties = context.ModelProperty.PropertyType.GetProperties();
			foreach (var naviQueryProperty in naviQueryProperties)
			{
				var naviContext = new AnalyzeContext()
					{
						Depth = context.Depth + 1,
						QueryProperty = naviQueryProperty,
						SourceParamName = context.SourceParamName,
						QueryParamName = naviQueryParam,
						Navigations = context.Navigations.Concat(new[] {context.ModelProperty.Name}).ToArray()
					};
				for (var index = 0; index < Analyzers.Length; index++)
				{
					var naviAnalyzer = Analyzers[index];
					var naviModelProperty = naviAnalyzer.FindAttachedModelProperty(naviQueryProperty, naviModelProperties);
					if (naviModelProperty == null) continue;
					naviContext.ModelProperty = naviModelProperty;
					var naviStatement = naviAnalyzer.ParseStatement(naviContext);
					queues[index].Add(naviStatement);
				}
				
			}
			for (var index = 0; index < Analyzers.Length; index++)
			{
				builder.Append(string.Join("", queues[index]));
			}
			builder.AppendLine("}");
			return builder.ToString();
		}

		public PropertyInfo FindAttachedModelProperty(PropertyInfo queryProperty, PropertyInfo[] modelProperties)
		{
			var queryPropertyName = queryProperty.Name;
			if (!queryPropertyName.EndsWith(Suffix)) return null;
			if (!TypeUtils.IsBaseEntityQuery(queryProperty.PropertyType)) return null;
			return modelProperties.FirstOrDefault(modelProperty => modelProperty.Name + Suffix == queryPropertyName);
		}
	}
}
