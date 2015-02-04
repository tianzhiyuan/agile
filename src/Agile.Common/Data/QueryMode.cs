using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Data
{
	/// <summary>
	/// Query setting
	/// </summary>
	public enum QueryMode
	{
		/// <summary>
		/// query result set and total count
		/// </summary>
		Both = 1,

		CountOnly = 2,
		ResultSetOnly = 3,
	}
}
