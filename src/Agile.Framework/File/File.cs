using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Framework.File
{
	
    /// <summary>
    /// 文件
    /// </summary>
    public class File
    {
		public string Handle { get; set; }
		public FileMetadata Metadata { get; set; }
    }
    public class FileMetadata
    {
		public FileMetadata()
		{
			Type = FileType.General;
		}
		public int Size { get; set; }
		public string Name { get; set; }
		public FileType Type { get; set; }
    }
}
