using System;
using Agile.Common.Data;

namespace Agile.Common.File
{
    /// <summary>
    /// 文件元数据
    /// </summary>
    [Serializable]
    public class FileMetadata : BaseEntity
    {
        /// <summary>
        /// 文件句柄
        /// </summary>
        public string FileHandle { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// mime type
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 大小 byte
        /// </summary>
        public long? Size { get; set; }
        /// <summary>
        /// md5
        /// </summary>
        public string Md5 { get; set; }
        /// <summary>
        /// File Type
        /// </summary>
        public int? FileType { get; set; }
    }
    [Serializable]
    public class FileMetadataQuery : BaseQuery<FileMetadata>
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string NamePattern { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public int? FileType { get; set; }

    }
}
