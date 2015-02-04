using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.File
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            ioc.RegisterType<IFileProcessorFactory, FileProcessorFactory>(LifeTimeManager.Singleton);

            ioc.RegisterType<IContentProcessor, WordProcessor>();
            ioc.RegisterType<IContentProcessor, ExcelProcessor>();
            ioc.RegisterType<IContentProcessor, ImageProcessor>();
            ioc.RegisterType<IContentProcessor, PdfProcessor>();
            ioc.RegisterType<IContentProcessor, PowerPoint2007Processor>();
            //ioc.RegisterType<IContentProcessor, PowerPointProsessor>("powerpoint");
            ioc.RegisterType<IContentProcessor, TiffProcessor>();
            ioc.RegisterType<IContentProcessor, YoutubeProcessor>();
            ioc.RegisterType<IContentProcessor, LinkProcessor>();
            ioc.RegisterType<IContentProcessor, VideoProcessor>();
            ioc.RegisterType<IContentProcessor, TextProcessor>();
            ioc.RegisterType<IContentProcessor, AudioProcessor>();
        }
    }                                                     
}                                                         
                                                          
                                                          