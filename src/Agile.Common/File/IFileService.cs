using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.File
{
    /// <summary>
    /// file service
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// create a file
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="filename">file origin name</param>
        /// <returns>file handle</returns>
        string Create(byte[] data, string filename);
        /// <summary>
        /// delete a file
        /// </summary>
        /// <param name="fileHandle"></param>
        /// <returns></returns>
        bool Delete(string fileHandle);
        /// <summary>
        /// get access url
        /// </summary>
        /// <param name="fileHandle"></param>
        /// <returns></returns>
        string GetAccessUri(string fileHandle);
        /// <summary>
        /// clone a file
        /// </summary>
        /// <param name="sourceFileHandle">source file handle</param>
        /// <returns>new file handle</returns>
        string Clone(string sourceFileHandle);
    }
}
