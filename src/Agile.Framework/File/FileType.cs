using System;
using System.ComponentModel;
using System.Linq;

namespace Agile.Framework.File
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FileTypeAcceptExtensionAttribute : Attribute
    {
        public FileTypeAcceptExtensionAttribute(string extensions)
        {
            if (string.IsNullOrWhiteSpace(extensions))
            {
                AcceptExtensions = new string[0];
            }
            else
            {
                AcceptExtensions =
                    extensions.Split(',')
                              .Where(o => !string.IsNullOrWhiteSpace(o))
                              .Select(o => o.Trim().ToLower())
                              .ToArray();
            }
        }

        public string[] AcceptExtensions { get; private set; }
    }
    public enum FileType
    {
        [Description("普通文件")]
        General = 1,
        [FileTypeAcceptExtension("jpg,bmp,jpeg,png")]
        [Description("图片")]
        Image = 2,
        [FileTypeAcceptExtension("doc,docx,xls,xlsx,pdf,txt,rtf")]
        [Description("文档")]
        Document = 3,
        [FileTypeAcceptExtension("mpg,mp4")]
        [Description("视频")]
        Video = 4,
        [Description("音频")]
        [FileTypeAcceptExtension("mp3,wav,rm")]
        Audio = 5,
    }
}
