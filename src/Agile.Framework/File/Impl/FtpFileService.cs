using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Logging;
using Agile.Framework.Data;

namespace Agile.Framework.File.Impl
{
	/// <summary>
	/// 基于ftp的文件存储服务
	/// </summary>
	public class FtpFileService : IFileService
	{
		#region private members

		private ILogger _logger;
		private IModelService _service;
		#endregion
		public string Username { get; set; }
		public string Password { get; set; }
		public string ServerAddr { get; set; }
		public string AccessUrlRoot { get; set; }
		public FtpFileService(ILoggerFactory factory, IModelService service)
		{
			this._logger = factory.Create(typeof (FtpFileService));
			this._service = service;
		}
		private FtpWebRequest CreateRequest(string relativePath)
		{
			var ftpRequest = (FtpWebRequest) WebRequest.Create(new UriBuilder(ServerAddr){Path = relativePath}.ToString());
			ftpRequest.Credentials = new NetworkCredential(Username, Password);
			ftpRequest.UseBinary = true;
			return ftpRequest;
		}
		private bool DirectoryExists(string relativePath)
		{
			var ftpRequest = CreateRequest(relativePath);
			ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
			try
			{
				using (var response = (FtpWebResponse) ftpRequest.GetResponse())
				using (var stream = new StreamReader(response.GetResponseStream()))
				{

				}
			}
			catch (WebException ex)
			{
				return false;
			}
			return true;
		}

		private void MakeSureDirectoryExist(string relativePath)
		{
			var segments = relativePath.Split(new char[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});
			int length = segments.Length;
			for (var index = 1; index < length; index++)
			{
				var directory = Path.Combine(segments.Take(index).ToArray());
				if (!DirectoryExists(directory))
				{
					MakeDirectory(directory);
				}
			}
		}
		private void MakeDirectory(string relativePath)
		{
			var ftpRequest = CreateRequest(relativePath);
			ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
			try
			{
				using (var response = (FtpWebResponse)ftpRequest.GetResponse())
				using (var stream = new StreamReader(response.GetResponseStream()))
				{

				}
			}
			catch (WebException ex)
			{
				
			}
		}
		private string GetRelativePath(string fileHandle)
		{
			return Path.Combine(fileHandle.Substring(0, 3),
			                    fileHandle.Substring(3, 4),
			                    fileHandle.Substring(7, 4),
			                    fileHandle.Substring(11));
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
			return (long)(current - new DateTime(current.Year, current.Month, 1)).TotalMilliseconds;
		}
		private static long GetMillisecondsOfADay(DateTime current)
		{
			return (long) (current - new DateTime(current.Year, current.Month, current.Day)).TotalMilliseconds;
		}
		
		#region Public Methods
		public string Create(byte[] data, string filename)
		{
			var extension = Path.GetExtension(filename);
			var fileType = FileTypeUtil.DeduceFileTypeFromExtension(extension);
			var now = DateTime.Now;
			//路径: [文件类型]/[年]/[月日]/[毫秒][随机串].后缀
			//文件句柄: 文件类型年月随机字符串.后缀
			//其中文件类型取类型名称的前三个字符，命名时应该注意！
			var fileTypeName = fileType.ToString().Substring(0, 3).PadLeft(3, '0');
			var year = string.Format("{0:yyyy}", now);
			var monthAndDay = string.Format("{0:MMdd}", now);
			//3+6+6+12
			var fileName = string.Format("{0}{1,12}{2}", Encode(GetMillisecondsOfADay(now)),
			                             Guid.NewGuid().ToString().Replace("-", ""),
			                             extension);
			var fileHandle = string.Format("{0}{1}{2}{3}", fileTypeName, year, monthAndDay, fileName);
			var file = new File()
			{
				Handle = fileHandle,
				Metadata = new FileMetadata()
				{
					Name = filename,
					Size = data.Length,
					Type = fileType
				}
			};
			//save it
			try
			{
				var directory = Path.Combine(fileTypeName, year, monthAndDay);
				if (!DirectoryExists(directory))
				{
					if (!DirectoryExists(Path.Combine(fileTypeName, year)))
					{
						if (!DirectoryExists(fileTypeName))
						{
							MakeDirectory(fileTypeName);
						}
						MakeDirectory(Path.Combine(fileTypeName, year));
					}
					MakeDirectory(directory);
				}
				var ftpRequest = CreateRequest(Path.Combine(directory, fileName));
				ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
				ftpRequest.ContentLength = data.Length;
				using (var requestStream = ftpRequest.GetRequestStream())
				{
					requestStream.Write(data, 0, data.Length);
				}
				using (var response = ftpRequest.GetResponse())
				using (var reader = new StreamReader(response.GetResponseStream()))
				{

				}
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
			var request = CreateRequest(GetRelativePath(fileHandle));
			//delete file from disk
			try
			{

				request.Method = WebRequestMethods.Ftp.DeleteFile;
				using(var response = request.GetResponse())
				using (var stream = new StreamReader(response.GetResponseStream()))
				{
					
				}
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
			var path = GetRelativePath(fileHandle);
			return new UriBuilder(AccessUrlRoot) { Path = path }.ToString();
		}

		public FileMetadata GetMetedata(string fileHandle)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
