using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;
            ioc.RegisterType<ILuisAi, LuisAi>(LifeTimeManager.Singleton);

            ioc.RegisterType<IIntent, HomeWorkIntent>("Homework");
            //ioc.RegisterType<IFileProcessorFactory, FileProcessorFactory>(LifeTimeManager.Singleton);

            //ioc.RegisterType<IContentProcessor, WordProcessor>();
            //ioc.RegisterType<IContentProcessor, ExcelProcessor>();
            //ioc.RegisterType<IContentProcessor, ImageProcessor>();
            //ioc.RegisterType<IContentProcessor, PdfProcessor>();
            //ioc.RegisterType<IContentProcessor, PowerPoint2007Processor>();
            //ioc.RegisterType<IContentProcessor, TiffProcessor>();
            //ioc.RegisterType<IContentProcessor, YoutubeProcessor>();
            //ioc.RegisterType<IContentProcessor, GoogleDriveProcessor>();
            //ioc.RegisterType<IContentProcessor, VideoProcessor>();
            //ioc.RegisterType<IContentProcessor, TextProcessor>();
            //ioc.RegisterType<IContentProcessor, AudioProcessor>();
            //ioc.RegisterType<IContentProcessor, LinkProcessor>();
            //ioc.RegisterType<IProfileProcessor, ImageProcessor>();
        }
    }
}
