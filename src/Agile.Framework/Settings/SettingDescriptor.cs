using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Settings
{
	public class SettingDescriptor
	{
		public PropertyInfo Property { get; private set; }

		public SettingDescriptor(PropertyInfo property)
		{
			Property = property;
			PropertyName = Property.Name;
			Description = "";
			ReadAttribute<DescriptionAttribute>(d => Description = d.Description);
			ReadAttribute<DefaultValueAttribute>(d => DefaultValue = d.Value);
		}
		public string PropertyName { get; set; }
		public string Description { get; set; }
		public object DefaultValue { get; set; }
		public virtual void SetValue(ISetting settings, object value)
		{
			Property.SetValue(settings, value);
		}
		public virtual object GetValue(ISetting settings)
		{
			var value = Property.GetValue(settings);
			return value;
		}

		protected void ReadAttribute<TAttribute>(Action<TAttribute> callback) where TAttribute : Attribute
		{
			var attribute = Property.GetCustomAttribute<TAttribute>(true);
			
			if (attribute != null)
			{
				callback(attribute);
			}
		}
	}
}
