using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Data
{
    public interface IOperator
    {
        /// <summary>
        /// 创建人
        /// </summary>
        int? CreatorId { get; set; }
        /// <summary>
        /// 最近修改人
        /// </summary>
        int? LastModifierId { get; set; }
    }
}
