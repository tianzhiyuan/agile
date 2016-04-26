using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common;
using Agile.Common.Logging;
using Agile.Common.Security;
using Agile.Common.Utils;
using Agile.Framework.Data;

namespace Agile.Framework.File.Impl
{
	/// <summary>
	/// 直接存储在应用本地的文件服务
	/// </summary>
	public class LocalFileService : IFileService
	{
		private readonly ILogger _logger;
		private static readonly DateTime baseDate = new DateTime(2000, 1, 1);
		private static long GetEpoch(DateTime dt)
		{
			return (long)(dt - baseDate).TotalMilliseconds;
		}
		private const string Clist = "0123456789abcdefghijklmnopqrstuvwxyz-_";
		private static readonly char[] Clistarr = Clist.ToCharArray();
		private string Encode(long inputNumber)
		{
			var sb = new StringBuilder();
			do
			{
				sb.Append(Clistarr[inputNumber % (long)Clist.Length]);
				inputNumber /= (long)Clist.Length;
			} while (inputNumber != 0);
			return sb.ToString();
		}
		private static long GetMillisecondsOfAMonth(DateTime current)
		{
			return (long) (current - new DateTime(current.Year, current.Month, 0)).TotalMilliseconds;
		}
        private static long GetMillisecondsOfADay(DateTime current)
        {
            return (long)(current - new DateTime(current.Year, current.Month, current.Day)).TotalMilliseconds;
        }
        public LocalFileService()
		{
			_logger = LoggerFactory.Create(typeof (LocalFileService));
			BaseDirectory = WebHelper.MapPath("~/uploadfiles");
		}
        /// <summary>
        /// 基础目录，默认为 ~/uploadfiles
        /// </summary>
		public string BaseDirectory { get; set; }
		public string AccessUrlRoot { get; set; }
		public string Create(byte[] content, string filename)
		{
			var extension = Path.GetExtension(filename);
			var fileType = FileTypeUtil.DeduceFileTypeFromExtension(extension);
			var now = DateTime.Now;
            //路径: [文件类型]/[年]/[月日]/[Encode[毫秒]][Encode[随机串]].后缀
            //文件句柄: 文件类型年月随机字符串.后缀
            //其中文件类型取类型名称的前五个字符，命名时应该注意！
            var rnd = BitConverter.ToInt64(Guid.NewGuid().ToByteArray().Take(48).ToArray(), 0);
            var fileTypeName = fileType.ToString().Substring(0, 5).PadLeft(5, '0').ToUpper();
            var year = string.Format("{0:yyyy}", now);
            var monthAndDay = string.Format("{0:MMdd}", now);

            var fileName = string.Format("{0}{1}{2}", Encode(GetMillisecondsOfADay(now)),
                Encode(rnd),
                extension);
            var fileHandle = string.Format("{0}{1}{2}{3}", fileTypeName, year, monthAndDay, fileName);
            var file = new FileMetadata()
				{
                FileHandle = fileHandle,
                Size = content.Length,
                Name = filename,
                CreatedAt = now,
                FileType = (int)fileType,
                Path = Path.Combine(fileTypeName, year, monthAndDay),
                MimeType = MimeTypeUtil.GetMimeType(extension)
            };
			//save file
		    var directory = Path.Combine(BaseDirectory, fileTypeName, year, monthAndDay);
			try
			{
				if (!Directory.Exists(directory))
				{
				    if (!Directory.Exists(Path.Combine(BaseDirectory, fileTypeName, year)))
				    {
				        if (!Directory.Exists(Path.Combine(BaseDirectory, fileTypeName)))
				        {
				            Directory.CreateDirectory(Path.Combine(BaseDirectory, fileTypeName));
				        }
				        Directory.CreateDirectory(Path.Combine(BaseDirectory, fileTypeName, year));
				    }
					Directory.CreateDirectory(directory);
				}
			}
			catch (Exception error)
			{
				_logger.Debug(string.Format("create directory error[{0}]", directory), error);
			}
			try
			{
				System.IO.File.Create(Path.Combine(directory, fileHandle));
				//TODO save metadata to database
			}
			catch (Exception error)
			{
				_logger.Error(string.Format("create error[{0}]", fileHandle), error);
				return null;
			}
			
			return fileHandle;
		}

		public bool Delete(string fileHandle)
		{
		    if (string.IsNullOrEmpty(fileHandle))
		    {
		        return false;
		    }
		    var path = GetRelativePath(fileHandle);
			//delete file from disk
			try
			{
			    System.IO.File.Delete(Path.Combine(BaseDirectory, path));
			}
			catch (Exception error)
			{
				_logger.Error(string.Format("delete error[{0}]", fileHandle), error);
				return false;
			}
			return true;
		}

	    public string GetAccessUri(string fileHandle)
	    {
            if (string.IsNullOrEmpty(fileHandle))
            {
                return "";
            }
            var path = GetRelativePath(fileHandle);
            return new UriBuilder(AccessUrlRoot) { Path = path }.ToString();
        }

	    public string Clone(string sourceFileHandle)
	    {
	        return sourceFileHandle;
	    }

	    public void Rename(string fileHandle, string newName)
		{
			
		}
        

        private string GetRelativePath(string fileHandle)
        {
            if (fileHandle.Length < 14)
            {
                return string.Empty;
            }
            return Path.Combine(fileHandle.Substring(0, 5),
                fileHandle.Substring(5, 4),
                fileHandle.Substring(9, 4),
                fileHandle.Substring(13));
        }
    }
}
