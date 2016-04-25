using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Utils
{
    public static class StringUtil
    {
        /// <summary>
        /// Retrieves a substring from this instance. The substring starts at a specified
        /// character position and has a specified length.
        /// 如果src为空，返回空字符串；如果src长度不足，返回从startIndex的最长字符串
        /// </summary>
        /// <param name="src">截断的字符串</param>
        /// <param name="startIndex">起始</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string Substring(string src, int startIndex, int length)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return string.Empty;
            }
            var strLen = src.Length;
            if (startIndex > strLen)
            {
                return string.Empty;
            }
            var endPos = Math.Min(startIndex + length, strLen);
            return src.Substring(startIndex, endPos - startIndex);
        }
        /// <summary>
        /// 返回指定长度的子串
        /// 如果src为空，返回空字符串，如果src长度不足，返回src的拷贝
        /// </summary>
        /// <param name="src"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Substring(string src, int length)
        {
            if (src == null)
            {
                return string.Empty;
            }
            var strLen = src.Length;
            var endPos = Math.Min(length, strLen);
            return src.Substring(0, endPos);
        }
    }
}
