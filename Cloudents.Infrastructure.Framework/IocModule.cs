using Autofac;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class IocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileFactoryProcessor>().AsImplementedInterfaces();
            builder.RegisterType<WordProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, WordProcessor.WordExtensions));
            builder.RegisterType<PdfProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PdfProcessor.PdfExtensions));
            builder.RegisterType<ImageProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ImageProcessor.ImageExtensions));
            builder.RegisterType<ExcelProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ExcelProcessor.ExcelExtensions));
            builder.RegisterType<PowerPoint2007Processor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PowerPoint2007Processor.PowerPoint2007Extensions));

            //builder.RegisterType<PdfProcessor>().Keyed()
            base.Load(builder);
        }
    }
}
