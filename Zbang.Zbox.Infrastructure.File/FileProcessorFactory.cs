﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        public static readonly string[] wordExtenstions = { "rtf", "docx", "doc", "html", "htm", "txt" };
        public static readonly string[] excelExtensions = { "xls", "xlsx", "xlsm", "xltx", "ods", "csv" };
        public static readonly string[] imageExtenstions = { "jpg", "gif", "png", "jpeg" };
        public static readonly string[] pdfExtenstions = { "pdf" };
        public static readonly string[] powerPoint2007Extenstions = { "pptx", "potx", "ppxs", "ppsx" };
        public static readonly string[] powerPointExtensions = { "ppt", "pot", "pps" };
        public static readonly string[] tiffExtesions = { "tiff", "tif" };
        public static readonly string[] linkExtension = { "link" };

        private readonly IEnumerable<IContentProcessor> m_Processors;
        public FileProcessorFactory()
        {
            m_Processors = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.ResolveAll<IContentProcessor>();
        }
        public IContentProcessor GetProcessor(Uri contentUrl)
        {
           
            var processor = m_Processors.FirstOrDefault(w => w.CanProcessFile(contentUrl));
            return processor;
            //var extension = ExtractExtension(fileName);
            //try
            //{
            //    return Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IFileProcessor>(extension);
            //}
            //catch
            //{
            //    return null;
            //}
        }

        //private string ExtractExtension(string fileName)
        //{
        //    Uri uri;
        //    if (Uri.TryCreate(fileName, UriKind.Absolute, out uri))
        //    {
        //        return "link";
        //    }
        //    var extension = Path.GetExtension(fileName);

        //    if (extension.StartsWith("."))
        //    {
        //        extension = extension.Remove(0, 1);
        //    }
        //    return extension.ToLower();
        //}
    }

}
