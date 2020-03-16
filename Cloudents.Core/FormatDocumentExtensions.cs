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

        private static IEnumerable<FileTypesExtension> GetTypes()
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

   
}