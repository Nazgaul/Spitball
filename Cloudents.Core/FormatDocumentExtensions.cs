using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cloudents.Core
{

    public class FileTypesExtension
    {
        public string DefaultThumbnail { get; }
        public string[] Extensions { get; }
        public DocumentType DocumentType { get; }

        private FileTypesExtension(string defaultThumbnail, string[] extensions, DocumentType documentType)
        {
            DefaultThumbnail = defaultThumbnail;
            Extensions = extensions;
            DocumentType = documentType;
        }

        public static FileTypesExtension Excel = new FileTypesExtension("Icons_720_excel.png",
            new[] { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" }, DocumentType.Document);

        public static FileTypesExtension Image = new FileTypesExtension("Icons_720_image.png",
            new[] { ".jpg", ".gif", ".png", ".jpeg", ".bmp" }, DocumentType.Document);

        public static FileTypesExtension Pdf = new FileTypesExtension("Icons_720_pdf.png",
            new[] { ".pdf" }, DocumentType.Document);

        public static FileTypesExtension PowerPoint = new FileTypesExtension("Icons_720_power.png",
            new[] { ".ppt", ".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm" }, DocumentType.Document);

        public static FileTypesExtension Text = new FileTypesExtension("Icons_720_txt.png",
            new[] { ".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html" }, DocumentType.Document);
        public static FileTypesExtension Tiff = new FileTypesExtension("Icons_720_image.png",
            new[] { ".tiff", ".tif" }, DocumentType.Document);

        public static FileTypesExtension Word = new FileTypesExtension("Icons_720_doc.png",
            new[] { ".rtf", ".docx", ".doc", ".odt" }, DocumentType.Document);

        public static FileTypesExtension Video = new FileTypesExtension("Icons_720_video.png",
            new[] {".flv", ".mxf", ".gxf", ".ts", ".ps", ".3gp", ".3gpp", ".mpg", ".wmv", ".asf", ".avi", ".mp4",
                ".m4v",  ".ismv", ".dvr-ms", ".mkv", ".mov",".webm" }, DocumentType.Video);

        public static FileTypesExtension Music = new FileTypesExtension("Icons_720_sound.png",
            new[] { ".wav", ".isma", ".m4a" }, DocumentType.Video);



    }

    public static class FileTypesExtensions
    {
        public static IEnumerable<string> GetFormats()
        {
            return GetTypes().Select(s => s.Extensions).SelectMany(z => z);
        }

        public static IEnumerable<FileTypesExtension> GetTypes()
        {
            return typeof(FileTypesExtension).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(w => !w.IsLiteral).Select(s => (FileTypesExtension)s.GetValue(null)).Where(w => w != null);
        }

        public static readonly Dictionary<string, FileTypesExtension> FileExtensionsMapping;

        static FileTypesExtensions()
        {
            FileExtensionsMapping = GetTypes()
                .SelectMany(v => v.Extensions, (parent, child) => new
                {
                    extension = child,
                    thumbnail = parent
                }).ToDictionary(x => x.extension, z => z.thumbnail, StringComparer.OrdinalIgnoreCase);
        }

    }

    //public static class FormatDocumentExtensions
    //{


    //    public const string ImageDefaultImage = "Icons_720_image.png";

    //    //[DefaultThumbnail("Icons_720_excel.png")]
    //    //public static readonly string[] Excel = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };
    //    //[DefaultThumbnail(ImageDefaultImage)]
    //    //public static readonly string[] Image = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };
    //    //[DefaultThumbnail("Icons_720_pdf.png")]
    //    //public static readonly string[] Pdf = { ".pdf" };
    //    //[DefaultThumbnail("Icons_720_power.png")]
    //    //public static readonly string[] PowerPoint = { ".ppt", ".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm" };
    //    //[DefaultThumbnail("Icons_720_txt.png")]
    //    //public static readonly string[] Text = { ".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html" };
    //    //[DefaultThumbnail(ImageDefaultImage)]
    //    //public static readonly string[] Tiff = { ".tiff", ".tif" };
    //    //[DefaultThumbnail("Icons_720_doc.png")]
    //    //public static readonly string[] Word = { ".rtf", ".docx", ".doc", ".odt" };
    //    //[DefaultThumbnail("Icons_720_video.png")]
    //    //public static readonly string[] Video =
    //    //{
    //    //    ".flv", ".mxf", ".gxf", ".ts", ".ps", ".3gp", ".3gpp", ".mpg", ".wmv", ".asf", ".avi", ".mp4", ".m4a",
    //    //    ".m4v",  ".ismv", ".dvr-ms", ".mkv", ".mov"
    //    //};

    //    [DefaultThumbnail("Icons_720_sound.png")]
    //    public static readonly string[] Music =
    //    {
    //        ".wav",".isma"
    //    };

    //    public static IEnumerable<string> GetFormats()
    //    {
    //        var b = GetTypes().Select(s => (string[]) s.GetValue(null));
    //        var t = b.SelectMany(z => z);
    //        return GetTypes().Select(s => (string[]) s.GetValue(null)).SelectMany(s => s);
    //    }

    //    public static IEnumerable<FieldInfo> GetTypes()
    //    {
    //        return typeof(FormatDocumentExtensions).GetFields(BindingFlags.Public | BindingFlags.Static)
    //            .Where(w => !w.IsLiteral);
    //    }

    //    public static void XXX<T>() where  T : Attribute
    //    {
    //        FormatDocumentExtensions.GetTypes().Select(s => new
    //        {
    //            extensions = (string[])s.GetValue(null),
    //            thumbnail = s.GetCustomAttribute<DefaultThumbnailAttribute>()?.Name ?? "doc-preview-empty.png"
    //        }).SelectMany(v => v.extensions, (parent, child) => new
    //        {
    //            extension = child,
    //            parent.thumbnail
    //        }).ToDictionary(x => x.extension, z => z.thumbnail);
    //    }

    //}

    //public class DefaultThumbnailAttribute : Attribute
    //{
    //    public DefaultThumbnailAttribute(string name)
    //    {
    //        Name = name;
    //    }

    //    public string Name { get; }
    //}
}