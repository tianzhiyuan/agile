using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.Tasks
{
    /// <summary>
    /// 任务接口
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// 唯一键
        /// </summary>
        Guid Key { get; }
        /// <summary>
        /// 是否可重入（同时有两个实例运行）
        /// ture-可重入；false-不可重入
        /// </summary>
        bool Reentrant { get; }
        /// <summary>
        /// 识别名称
        /// </summary>
        string FriendlyName { get; }
        /// <summary>
        /// 执行体
        /// </summary>
        /// <param name="context">任务执行上下文</param>
        void Execute(TaskExecutionContext context);
    }
}
