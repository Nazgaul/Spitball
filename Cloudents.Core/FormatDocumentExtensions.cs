using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cloudents.Core
{
    public static class FormatDocumentExtensions
    {
        public const string ImageDefaultImage = "Icons_720_image.png";

        [DefaultThumbnail("Icons_720_excel.png")]
        public static readonly string[] Excel = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };
        [DefaultThumbnail(ImageDefaultImage)]
        public static readonly string[] Image = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };
        [DefaultThumbnail("Icons_720_pdf.png")]
        public static readonly string[] Pdf = { ".pdf" };
        [DefaultThumbnail("Icons_720_power.png")]
        public static readonly string[] PowerPoint = { ".ppt", ".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm" };
        [DefaultThumbnail("Icons_720_txt.png")]
        public static readonly string[] Text = { ".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html" };
        [DefaultThumbnail(ImageDefaultImage)]
        public static readonly string[] Tiff = { ".tiff", ".tif" };
        [DefaultThumbnail("Icons_720_doc.png")]
        public static readonly string[] Word = { ".rtf", ".docx", ".doc", ".odt" };

        public static readonly string[] Video =
        {
            ".flv", ".mxf", ".gxf", ".ts", ".ps", ".3gp", ".3gpp", ".mpg", ".wmv", ".asf", ".avi", ".mp4", ".m4a",
            ".m4v", ".isma", ".ismv", ".dvr-ms", ".mkv", ".wav", ".mov"
        };

        public static IEnumerable<string> GetFormats()
        {
            var b = GetTypes().Select(s => (string[]) s.GetValue(null));
            var t = b.SelectMany(z => z);
            return GetTypes().Select(s => (string[]) s.GetValue(null)).SelectMany(s => s);
        }

        public static IEnumerable<FieldInfo> GetTypes()
        {
            return typeof(FormatDocumentExtensions).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(w => !w.IsLiteral);
        }

    }

    public class DefaultThumbnailAttribute : Attribute
    {
        public DefaultThumbnailAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}