using Autofac;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Infrastructure.Framework
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Autofac module")]
    public class ModuleFile : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileFactoryProcessor>().AsImplementedInterfaces();
            builder.RegisterType<WordProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, WordProcessor.Extensions));
            builder.RegisterType<PdfProcessor>().AsSelf().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PdfProcessor.Extensions));
            builder.RegisterType<ImageProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ImageProcessor.Extensions));
            //builder.RegisterType<ImageProcessor>().As<IBlurProcessor>();
            builder.RegisterType<ExcelProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ExcelProcessor.Extensions));
            builder.RegisterType<PowerPoint2007Processor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PowerPoint2007Processor.Extensions));
            builder.RegisterType<TextProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TextProcessor.Extensions));
            builder.RegisterType<TiffProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TiffProcessor.Extensions));
            base.Load(builder);
        }
    }
}
