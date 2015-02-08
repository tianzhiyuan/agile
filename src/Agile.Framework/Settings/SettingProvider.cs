using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using Agile.Common.Logging;
using Agile.Framework.Caching;
using Agile.Framework.Data;

namespace Agile.Framework.Settings
{
	public class SettingProvider : ISettingProvider
	{
		
		private ILogger _logger;
		private IModelService _modelService;
		private ICache _cache;
		public SettingProvider(ILoggerFactory factory, IModelService service, ICache cacheProvider)
		{
			_logger = factory.Create(typeof (SettingProvider));
			_modelService = service;
			_cache = cacheProvider;
		}

		public TSetting Get<TSetting>(string appName = null) where TSetting : class, ISetting, new() 
		{
			if (string.IsNullOrWhiteSpace(appName))
			{
				appName = SettingConstant.DefaultAppName;
			}
			//read from cache
			var settingCacheObj = _cache.Get(SettingConstant.SettingCacheKey) as IDictionary<string, ISetting>;
			var settingType = typeof(TSetting);
			var settingTypeName = string.Format("{0}.{1}", appName, settingType.FullName);
			return (TSetting) settingCacheObj[settingTypeName];
		}

		public void Save<TSetting>(TSetting settings, string appName = null) where TSetting : class, ISetting, new()
		{
			if (string.IsNullOrWhiteSpace(appName))
			{
				appName = SettingConstant.DefaultAppName;
			}
			var settingType = settings.GetType();
			var settingTypeName = string.Format("{0}.{1}", appName, settingType.FullName);
			var appSettingProperties =
				_modelService.Select(new AppSettingQuery() {SettingType = settingTypeName, Mode = QueryMode.ResultSetOnly});
			if (!appSettingProperties.Any())
			{
				//create
				appSettingProperties = settingType
					.GetProperties()
					.Where(o => SettingHelper.Supported(o.PropertyType))
					.Select(o =>
						{
							var setting = new AppSetting()
								{
									SettingType = settingTypeName,
									PropertyName = o.Name,
								};
							var value = o.GetValue(settings);

							setting.PropertyValue = Convert.ToString(value);

							return setting;
						});
				_modelService.Create(appSettingProperties.ToArray());
			}
			else
			{
				//update
				var properties = settingType.GetProperties();
				foreach (var settingProperty in appSettingProperties)
				{
					var property = properties.FirstOrDefault(o => o.Name == settingProperty.PropertyName);
					if (property == null) continue;
					settingProperty.PropertyValue = Convert.ToString(property.GetValue(settings));
				}
			}
			
			//update cache
		}

		protected IEnumerable<SettingDescriptor> GetDescriptors(Type settingType)
		{
			var properties = settingType.GetProperties().Where(o => SettingHelper.Supported(o.PropertyType));
			return properties.Select(o => new SettingDescriptor(o)).ToArray();
		}

	}
}
