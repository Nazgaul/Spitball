using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    public class ModuleFile : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileFactoryProcessor>().AsImplementedInterfaces();
            builder.RegisterType<WordProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, WordProcessor.WordExtensions));
            builder.RegisterType<PdfProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PdfProcessor.PdfExtensions));
            builder.RegisterType<ImageProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ImageProcessor.ImageExtensions));
            builder.RegisterType<ExcelProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ExcelProcessor.ExcelExtensions));
            builder.RegisterType<PowerPoint2007Processor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PowerPoint2007Processor.PowerPoint2007Extensions));
            builder.RegisterType<TextProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TextProcessor.TxtExtensions));
            builder.RegisterType<TiffProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TiffProcessor.TiffExtensions));
            base.Load(builder);
        }
    }
}
