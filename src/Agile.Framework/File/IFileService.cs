using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.File
{
    /// <summary>
    /// 文件服务接口
    /// </summary>
    public interface IFileService
    {
		/// <summary>
		/// 创建文件
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="filename">文件名称</param>
		/// <returns>文件句柄</returns>
        string Create(byte[] data, string filename);
		/// <summary>
		/// 删除文件
		/// </summary>
		/// <param name="fileHandle">文件句柄</param>
		/// <returns></returns>
		bool Delete(string fileHandle);
		/// <summary>
		/// 重命名元数据中的文件名
		/// </summary>
		/// <param name="fileHandle">文件句柄</param>
		/// <param name="newName">新名称</param>
	    void Rename(string fileHandle, string newName);
		/// <summary>
		/// 获取访问的url
		/// </summary>
		/// <param name="fileHandle">文件句柄</param>
		/// <returns></returns>
		string GetAccessUrl(string fileHandle);
    }
}
