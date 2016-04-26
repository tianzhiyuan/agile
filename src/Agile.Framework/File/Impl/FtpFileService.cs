using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Agile.Common.Components;
using Agile.Common.Logging;
using Agile.Common.Utils;
using Agile.Framework.Data;

namespace Agile.Framework.File.Impl
{
    /// <summary>
    /// 基于ftp的文件存储服务
    /// </summary>
    [Component]
    public class FtpFileService : IFileService
    {
        #region private members

        private readonly ILogger _logger;
        private readonly IFtpConfigProvider _configProvider;
        #endregion

        public FtpFileService(IFtpConfigProvider configProvider)
        {
            this._logger = LoggerFactory.Create(typeof(FtpFileService));
            _configProvider = configProvider;
        }

        #region private methods
        private FtpWebRequest CreateRequest(string relativePath)
        {
            var ftpRequest =
                (FtpWebRequest)WebRequest.Create(new UriBuilder(_configProvider.ServerAddr) { Path = relativePath }.ToString());
            ftpRequest.Credentials = new NetworkCredential(_configProvider.Username, _configProvider.Password);
            ftpRequest.UseBinary = true;
            return ftpRequest;
        }

        private bool DirectoryExists(string relativePath)
        {
            var ftpRequest = CreateRequest(relativePath);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            try
            {
                using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    var content = stream.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return false;
                    }
                }
            }
            catch (WebException)
            {
                return false;
            }
            return true;
        }

        private void MakeSureDirectoryExist(string relativePath)
        {
            var segments = relativePath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
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
            catch (WebException)
            {

            }
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

        private const string _Clist = "0123456789abcdefghijklmnopqrstuvwxyz-_";
        private static readonly char[] _Clistarr = _Clist.ToCharArray();
        private static readonly int _CLength = _Clistarr.Length;
        private string Encode(long inputNumber)
        {
            var sb = new StringBuilder();
            do
            {
                sb.Append(_Clistarr[inputNumber % (long)_CLength]);
                inputNumber /= (long)_Clist.Length;
            } while (inputNumber != 0);
            return sb.ToString();
        }

        private static long GetMillisecondsOfAMonth(DateTime current)
        {
            return (long)(current - new DateTime(current.Year, current.Month, 1)).TotalMilliseconds;
        }

        private static long GetMillisecondsOfADay(DateTime current)
        {
            return (long)(current - new DateTime(current.Year, current.Month, current.Day)).TotalMilliseconds;
        }
        #endregion
        #region Public Methods

        public string Create(byte[] data, string filename)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException("data");
            }

            var extension = Path.GetExtension(filename);
            var fileType = FileTypeUtil.DeduceFileTypeFromExtension(extension);
            var now = DateTime.Now;
            //路径: [文件类型]/[年]/[月日]/[Encode[毫秒]][Encode[随机串]].后缀
            //例如：IMAGE/2016/0314/Xb4J3ims7gC4Qyk4.jpg
            //注意：文件名区分大小写，必须使用Linux文件系统
            //文件句柄: 文件类型年月随机字符串.后缀
            //其中文件类型取类型名称的前五个字符，命名时应该注意！

            //c# GUID目前采用算法4 即前12位是随机数
            //xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx
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
                Size = data.Length,
                Name = filename,
                CreatedAt = now,
                FileType = (int)fileType,
                Path = Path.Combine(fileTypeName, year, monthAndDay),
                MimeType = MimeTypeUtil.GetMimeType(extension)
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
                try
                {
                    var md5 = new MD5CryptoServiceProvider().ComputeHash(data);
                    file.Md5 = BitConverter.ToString(md5).Replace("-", string.Empty);
                    //todo insert file meta
                }
                catch (System.Exception error)
                {
                    _logger.Error(string.Format("save filemetadata{0} error", fileHandle), error);
                }
            }
            catch (System.Exception error)
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
                using (var response = request.GetResponse())
                using (var stream = new StreamReader(response.GetResponseStream()))
                {

                }
                //todo delete file meta
            }
            catch (System.Exception error)
            {
                _logger.Error(string.Format("delete error[{0}]", fileHandle), error);
                return false;
            }
            return true;
        }

        public void Rename(string fileHandle, string newName)
        {
            //todo change file meta
        }

        public string GetAccessUri(string fileHandle)
        {
            if (string.IsNullOrEmpty(fileHandle))
            {
                return "";
            }
            var path = GetRelativePath(fileHandle);
            return new UriBuilder(_configProvider.AccessUrlRoot) { Path = path }.ToString();
        }

        public string Clone(string sourceFileHandle)
        {
            if (string.IsNullOrWhiteSpace(sourceFileHandle))
            {
                throw new ArgumentNullException("sourceFileHandle");
            }
            var request = CreateRequest(GetRelativePath(sourceFileHandle));
            try
            {
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                using (var response = request.GetResponse())
                using (var stream = (response.GetResponseStream()))
                using (var memory = new MemoryStream())
                {
                    
                    stream.CopyTo(memory);
                    return this.Create(memory.ToArray(), "");
                }
            }
            catch (System.Exception error)
            {
                _logger.Error("error when clone. source file handle:" + sourceFileHandle, error);
                return null;
            }
        }

        #endregion
    }
}
