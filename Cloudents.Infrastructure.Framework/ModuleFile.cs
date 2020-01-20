using Autofac;
using Cloudents.Core;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Infrastructure.Framework
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Autofac module")]
    public class ModuleFile : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileFactoryProcessor>().AsImplementedInterfaces();
            builder.RegisterType<WordProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, FileTypesExtension.Word.Extensions));
            builder.RegisterType<PdfProcessor>().AsSelf().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, FileTypesExtension.Pdf.Extensions));
            builder.RegisterType<ImageProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, FileTypesExtension.Image.Extensions));
            builder.RegisterType<ExcelProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, FileTypesExtension.Excel.Extensions));
            builder.RegisterType<PowerPoint2007Processor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, FileTypesExtension.PowerPoint.Extensions));
            builder.RegisterType<TextProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, FileTypesExtension.Text.Extensions));
            builder.RegisterType<TiffProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, FileTypesExtension.Tiff.Extensions));
            base.Load(builder);
        }
    }
}
