using System;
using System.Collections.Generic;

namespace Agile.Common.File
{
    public class FileTypeUtil
    {
        private static readonly IDictionary<string, FileType> FileTypeExtensionMapping;
        static FileTypeUtil()
        {
            var mapping = new Dictionary<string, FileType>();
            var enumType = typeof(FileType);
            foreach (var enumField in (FileType[])Enum.GetValues(enumType))
            {
                var member = enumType.GetMember(enumField.ToString())[0];
                var attribute = (FileTypeAcceptExtensionAttribute)Attribute.GetCustomAttribute(member, typeof(FileTypeAcceptExtensionAttribute));
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

        public static FileType DeduceFileTypeFromExtension(string fileExt)
        {
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return FileType.General;
            }
            fileExt = fileExt.TrimStart('.').ToLower();
            FileType type;
            if (FileTypeExtensionMapping.TryGetValue(fileExt, out type))
            {
                return type;
            }
            return FileType.General;
        }
    }
}
