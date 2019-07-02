﻿namespace Cloudents.Core
{
    public static class FormatDocumentExtensions
    {
        public static readonly string[] Excel = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };
        public static readonly string[] Image = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };
        public static readonly string[] Pdf = { ".pdf" };
        public static readonly string[] PowerPoint = { ".ppt", ".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm" };
        public static readonly string[] Text = { ".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html" };
        public static readonly string[] Tiff = { ".tiff", ".tif" };
        public static readonly string[] Word = { ".rtf", ".docx", ".doc", ".odt" };
    }
}