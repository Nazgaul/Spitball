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

            //builder.RegisterType<PdfProcessor>().Keyed()
            base.Load(builder);
        }
    }
}
