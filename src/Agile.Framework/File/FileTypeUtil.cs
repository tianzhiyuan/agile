using System;
using System.Collections.Generic;

namespace Agile.Framework.File
{
    public class FileTypeUtil
    {
        private static readonly IDictionary<string, Framework.File.FileType> FileTypeExtensionMapping;
        static FileTypeUtil()
        {
            var mapping = new Dictionary<string, Framework.File.FileType>();
            var enumType = typeof(Framework.File.FileType);
            foreach (var enumField in (Framework.File.FileType[])Enum.GetValues(enumType))
            {
                var member = enumType.GetMember(enumField.ToString())[0];
                var attribute = (Framework.File.FileTypeAcceptExtensionAttribute)Attribute.GetCustomAttribute(member, typeof(Framework.File.FileTypeAcceptExtensionAttribute));
                if (attribute != null)
                {
                    foreach (var ext in attribute.AcceptExtensions)
                    {
                        if (!mapping.ContainsKey(ext))
                        {
                            mapping.Add(ext, enumField);
                        }
                    }
                }
            }
            FileTypeExtensionMapping = mapping;
        }

        public static Framework.File.FileType DeduceFileTypeFromExtension(string fileExt)
        {
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return Framework.File.FileType.General;
            }
            fileExt = fileExt.TrimStart('.').ToLower();
            Framework.File.FileType type;
            if (FileTypeExtensionMapping.TryGetValue(fileExt, out type))
            {
                return type;
            }
            return Framework.File.FileType.General;
        }
    }
}
