using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.DataConfig
{
    /// <summary>
    /// 元数据
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// 实体的属性信息
        /// </summary>
        public IEnumerable<Property> Properties { get; set; }
        /// <summary>
        /// 键
        /// </summary>
        public Property Key { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public IEnumerable<string> Columns { get; set; }
    }
}
