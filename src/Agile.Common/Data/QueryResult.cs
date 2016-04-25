using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Data
{
    [Serializable]
    public class QueryResult<T>
    {
        public IEnumerable<T> List { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int Count { get; set; }
    }
}
