using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.File
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            ioc.RegisterType<IFileProcessorFactory, FileProcessorFactory>(LifeTimeManager.Singleton);

            ioc.RegisterType<IContentProcessor, WordProcessor>("word");
            ioc.RegisterType<IContentProcessor, ExcelProcessor>("excel");
            ioc.RegisterType<IContentProcessor, ImageProcessor>("image");
            ioc.RegisterType<IContentProcessor, PdfProcessor>("pdf");
            ioc.RegisterType<IContentProcessor, PowerPoing2007Processor>("powerpoint2007");
            //ioc.RegisterType<IContentProcessor, PowerPointProsessor>("powerpoint");
            ioc.RegisterType<IContentProcessor, TiffProcessor>("tiff");
            ioc.RegisterType<IContentProcessor, YoutubeProcessor>("youtube");
            ioc.RegisterType<IContentProcessor, LinkProcessor>("link");
            ioc.RegisterType<IContentProcessor, VideoProcessor>("video");
            ioc.RegisterType<IContentProcessor, HtmlProcessor>("html");
        }
    }                                                     
}                                                         
                                                          
                                                          