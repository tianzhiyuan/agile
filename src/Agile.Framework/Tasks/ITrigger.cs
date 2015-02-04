using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Tasks
{
    /// <summary>
    /// 触发器接口
    /// 触发器相关的时间都会选择UTC时间，排除时区的影响
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// 触发时回调
        /// </summary>
        event EventHandler Triggered;
        /// <summary>
        /// 唯一键
        /// </summary>
        Guid Key { get; }
        /// <summary>
        /// 识别名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 上次触发的UTC时间
        /// 如果还未触发则返回null
        /// </summary>
        /// <returns></returns>
        DateTime? GetLastFireTimeUtc();
        /// <summary>
        /// 下次触发预计的UTC时间
        /// 如果该触发器已经停止或者禁用则返回null
        /// </summary>
        /// <returns></returns>
        DateTime? GetNextFireTimeUtc();
        /// <summary>
        /// 是否启用
        /// </summary>
        bool Enabled { get; }
        /// <summary>
        /// 启用触发器，供<see cref="IScheduler"/>调用
        /// </summary>
        void Enable();
        /// <summary>
        /// 禁用触发器
        /// </summary>
        void Disable();
    }
}
