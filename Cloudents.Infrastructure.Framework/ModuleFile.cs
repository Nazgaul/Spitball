using Autofac;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core;

namespace Cloudents.Infrastructure.Framework
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Autofac module")]
    public class ModuleFile : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileFactoryProcessor>().AsImplementedInterfaces();
            builder.RegisterType<WordProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, WordProcessor.Extensions));
            builder.RegisterType<PdfProcessor>().AsSelf().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PdfProcessor.Extensions));
            builder.RegisterType<ImageProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ImageProcessor.Extensions));
            builder.RegisterType<ExcelProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ExcelProcessor.Extensions));
            builder.RegisterType<PowerPoint2007Processor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PowerPoint2007Processor.Extensions));
            builder.RegisterType<TextProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TextProcessor.Extensions));
            builder.RegisterType<TiffProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TiffProcessor.Extensions));
            base.Load(builder);
        }
    }
}
