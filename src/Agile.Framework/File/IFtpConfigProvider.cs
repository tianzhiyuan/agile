using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.File
{
    /// <summary>
    /// ftp 配置provider 接口
    /// </summary>
    public interface IFtpConfigProvider
    {
        string Username { get; }
        string Password { get; }
        string ServerAddr { get; }
        string AccessUrlRoot { get; }
    }
}
