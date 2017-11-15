using Autofac;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    //public static class RegisterIoc
    //{
    //    public static void Register()
    //    {
    //        var ioc = IocFactory.IocWrapper;
    //        ioc.RegisterType<IFileProcessorFactory, FileProcessorFactory>(LifeTimeManager.Singleton);

    //        ioc.RegisterType<IContentProcessor, WordProcessor>();
    //        ioc.RegisterType<IContentProcessor, ExcelProcessor>();
    //        ioc.RegisterType<IContentProcessor, ImageProcessor>();
    //        ioc.RegisterType<IContentProcessor, PdfProcessor>();
    //        ioc.RegisterType<IContentProcessor, PowerPoint2007Processor>();
    //        ioc.RegisterType<IContentProcessor, TiffProcessor>();
    //        ioc.RegisterType<IContentProcessor, YoutubeProcessor>();
    //        ioc.RegisterType<IContentProcessor, GoogleDriveProcessor>();
    //        ioc.RegisterType<IContentProcessor, VideoProcessor>();
    //        ioc.RegisterType<IContentProcessor, TextProcessor>();
    //        ioc.RegisterType<IContentProcessor, AudioProcessor>();
    //        ioc.RegisterType<IContentProcessor, LinkProcessor>();
    //        //ioc.RegisterType<IProfileProcessor, ImageProcessor>();
    //    }
    //}

    public class FileModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<IFileProcessorFactory, FileProcessorFactory>(LifeTimeManager.Singleton);
            builder.RegisterType<FileProcessorFactory>().As<IFileProcessorFactory>().SingleInstance();
            builder.RegisterType<IContentProcessor, WordProcessor>();
            builder.RegisterType<IContentProcessor, ExcelProcessor>();
            builder.RegisterType<IContentProcessor, ImageProcessor>();
            builder.RegisterType<IContentProcessor, PdfProcessor>();
            builder.RegisterType<IContentProcessor, PowerPoint2007Processor>();
            builder.RegisterType<IContentProcessor, TiffProcessor>();
            builder.RegisterType<IContentProcessor, YoutubeProcessor>();
            builder.RegisterType<IContentProcessor, GoogleDriveProcessor>();
            builder.RegisterType<IContentProcessor, VideoProcessor>();
            builder.RegisterType<IContentProcessor, TextProcessor>();
            builder.RegisterType<IContentProcessor, AudioProcessor>();
            builder.RegisterType<IContentProcessor, LinkProcessor>();
        }
    }
}

