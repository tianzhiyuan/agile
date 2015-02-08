using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Agile.Common;
using Agile.Common.Data;
using Agile.Common.Logging;
using Agile.Framework.Caching;
using Agile.Framework.Data;

namespace Agile.Framework.Settings
{
	public class SettingProvider : ISettingProvider, IAssemblyInitializer
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
			var settingCacheObj = Get(appName);
			var settingType = typeof(TSetting);
			var settingTypeName = string.Format("{0}.{1}", appName, settingType.FullName);
			return (TSetting) settingCacheObj[settingTypeName];
		}

		public void Save<TSetting>(TSetting settings, string appName = null) where TSetting : class, ISetting
		{
			if (string.IsNullOrWhiteSpace(appName))
			{
				appName = SettingConstant.DefaultAppName;
			}
			var settingType = settings.GetType();
			var settingTypeName = GetSettingTypeName(appName, settingType);
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
					try
					{
						settingProperty.PropertyValue = Convert.ToString(property.GetValue(settings));
					}
					catch (Exception error)
					{
						
					}
				}
			}
			
			//update cache
			Get(appName, true);
			
		}

		protected IEnumerable<SettingDescriptor> GetDescriptors(Type settingType)
		{
			var properties = settingType.GetProperties().Where(o => SettingHelper.Supported(o.PropertyType));
			return properties.Select(o => new SettingDescriptor(o)).ToArray();
		}
		protected IDictionary<string, ISetting> Get(string appName, bool forceToRefresh = false)
		{
			var cacheObj = _cache.Get(SettingConstant.SettingCacheKey) as IDictionary<string, ISetting>;
			if (cacheObj == null || forceToRefresh)
			{
				var allSettings = _modelService.Select(new AppSettingQuery() {Mode = QueryMode.ResultSetOnly});
				cacheObj = new Dictionary<string, ISetting>();
				if (!allSettings.Any())
				{
					_cache.AddOrUpdate(SettingConstant.SettingCacheKey, 10);
				}
				else
				{
					var lookup = allSettings.ToLookup(o => o.SettingType);
					foreach (var settingProperties in lookup)
					{
						var settingType = Type.GetType(settingProperties.Key.Substring(appName.Length + 1));
						if (settingType == null)
						{
							_logger.DebugFormat("no such setting type:{0}", settingProperties.Key);
							continue;
						}
						var defaultInstance = SettingHelper.ConstructDefault(settingType);
						var propertyValues = lookup[settingProperties.Key];
						foreach (var propertyValue in propertyValues)
						{
							var propertyInfo = settingType.GetProperty(propertyValue.PropertyName);
							if (propertyInfo == null)
							{
								_logger.DebugFormat("no property named '{0}' in setting type '{1}'", propertyValue.PropertyName,
								                    settingProperties.Key);
								continue;
							}
							try
							{
								propertyInfo.SetValue(defaultInstance,
								                      Convert.ChangeType(propertyValue.PropertyValue, propertyInfo.PropertyType));
							}
							catch (Exception error)
							{
								_logger.Debug("", error);
							}
						}
						cacheObj.Add(settingProperties.Key, defaultInstance);
					}
					_cache.AddOrUpdate(SettingConstant.SettingCacheKey, 30);
				}
			}
			return cacheObj;
		} 
		protected string GetSettingTypeName(string appName, Type settingType)
		{
			return string.Format("{0}.{1}", appName, settingType.FullName);
		}
		
		protected bool ExistInStore(string appName, Type settingType)
		{
			return _modelService.Any(new AppSettingQuery()
				{
					SettingType = GetSettingTypeName(appName, settingType)
				});
		}
		

		public void Initialize(Assembly[] assemblies)
		{
			var settingTypes = new List<Type>();
			foreach (var assembly in assemblies)
			{
				try
				{
					foreach (var type in assembly.GetTypes())
					{
						if (typeof(ISetting).IsAssignableFrom(type))
						{
							settingTypes.Add(type);
						}
					}
				}
				catch (Exception error)
				{
					_logger.Error("error in settingprovider", error);
				}
			}
			var settingsDefault = settingTypes.Select(SettingHelper.ConstructDefault);
			foreach (var setting in settingsDefault)
			{
				if (!ExistInStore(SettingConstant.DefaultAppName, setting.GetType()))
				{
					Save(setting);
				}
			}
		}
	}
}
