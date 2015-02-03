using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Install
{
	public interface IInstallService
	{
		void Install(bool installSampleData);
	}
}
