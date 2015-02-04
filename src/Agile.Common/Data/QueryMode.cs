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
		/// <summary>
		/// only query total count
		/// </summary>
		CountOnly = 2,
		/// <summary>
		/// only query result set
		/// </summary>
		ResultSetOnly = 3,
	}
}
