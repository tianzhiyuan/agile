using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;

namespace Agile.Framework.Settings
{
	[Serializable]
	public class AppSetting : BaseEntity
	{
		public string SettingType { get; set; }
		public string PropertyName { get; set; }
		public string PropertyValue { get; set; }
	}
	public class AppSettingQuery:BaseQuery<AppSetting>
	{
		public string SettingType { get; set; }
		public string PropertyName { get; set; }
	}
}
