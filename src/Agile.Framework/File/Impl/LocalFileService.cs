using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common;
using Agile.Common.Logging;
using Agile.Common.Security;
using Agile.Framework.Data;

namespace Agile.Framework.File.Impl
{
	/// <summary>
	/// 直接存储在应用本地的文件服务
	/// </summary>
	public class LocalFileService : IFileService
	{
		private IModelService _dataService;
		private ILogger _logger;
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
		public LocalFileService(IModelService service, ILoggerFactory factory)
		{
			_dataService = service;
			_logger = factory.Create(typeof (LocalFileService));
			BaseDirectory = WebHelper.MapPath("~/uploadfiles");
		}
		public string BaseDirectory { get; set; }
		public string AccessUrlRoot { get; set; }
		public string Create(byte[] content, string filename)
		{
			var extension = Path.GetExtension(filename);
			var fileType = FileTypeUtil.DeduceFileTypeFromExtension(extension);
			var now = DateTime.Now;
			//路径: [文件类型]/[年月]/文件句柄.后缀
			//文件句柄: 文件类型年月随机字符串.后缀
			//其中文件类型取类型名称的前三个字符，命名时应该注意！
			var fileTypeName = fileType.ToString().Substring(0, 3).PadLeft(3, '0');
			var middleName = string.Format("{0:yyyyMM}", DateTime.Now);
			//3+6+6+12
			var rand = string.Format("{0}{1,12}", Encode(GetMillisecondsOfAMonth(now)),
									 Guid.NewGuid().ToString().Replace("-", ""));
			var fileHandle = string.Format("{0}{1}{2}{3}", fileTypeName, middleName, rand, extension);
			var file = new File()
				{
					Handle = fileHandle,
					Metadata = new FileMetadata()
						{
							Name = filename,
							Size = content.Length,
							Type = fileType
						}
				};
			//save file
			var directory = Path.Combine(BaseDirectory, fileTypeName, middleName);
			try
			{
				if (!Directory.Exists(directory))
				{
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
			var path = Path.Combine(BaseDirectory, fileHandle.Substring(0, 3), fileHandle.Substring(3, 6), fileHandle);
			//delete file from disk
			try
			{
				System.IO.File.Delete(path);
			}
			catch (Exception error)
			{
				_logger.Error(string.Format("delete error[{0}]", fileHandle), error);
				return false;
			}
			return true;
		}

		public void Rename(string fileHandle, string newName)
		{
			throw new NotImplementedException();
		}

		public string GetAccessUrl(string fileHandle)
		{
			var path = string.Format("/{0}/{1}/{2}", fileHandle.Substring(0, 3), fileHandle.Substring(3, 6), fileHandle);
			return new UriBuilder(AccessUrlRoot) {Path = path}.ToString();
		}

		public FileMetadata GetMetedata(string fileHandle)
		{
			throw new NotImplementedException();
		}
	}
}
