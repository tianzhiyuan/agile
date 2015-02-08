using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Settings
{
	public interface ISettingProvider
	{
		TSetting Get<TSetting>(string appName = null) where TSetting : class, ISetting, new();
		void Save<TSetting>(TSetting settings, string appName = null) where TSetting : class, ISetting;
	}
	
}
