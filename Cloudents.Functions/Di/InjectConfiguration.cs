using Autofac;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Cloudents.Functions.Di
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var services = new ContainerBuilder();
            RegisterServices(services);
            var container = services.Build();
            //var serviceProvider = services.BuildServiceProvider(true);

            context
                .AddBindingRule<InjectAttribute>()
                .Bind(new InjectBindingProvider(container));

            var registry = context.Config.GetService<IExtensionRegistry>();
            var filter = new ScopeCleanupFilter();
            registry.RegisterExtension(typeof(IFunctionInvocationFilter), filter);
            registry.RegisterExtension(typeof(IFunctionExceptionFilter), filter);
        }
        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterModule<ModuleFile>();
            //builder.RegisterType<FileProcessorFactory>().AsImplementedInterfaces();


            //builder.RegisterType<VideoProcessor>().As<IFileProcessor>()
            //    .WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName,
            //        FormatDocumentExtensions.Video));


            //builder.RegisterType<DocumentProcessor>().As<IFileProcessor>()
            //    .WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName,
            //        FormatDocumentExtensions.Word
            //            .Union(FormatDocumentExtensions.Excel)
            //            .Union(FormatDocumentExtensions.Tiff)
            //            .Union(FormatDocumentExtensions.Image)
            //            .Union(FormatDocumentExtensions.PowerPoint)
            //            .Union(FormatDocumentExtensions.Text)
            //            .Union(FormatDocumentExtensions.Pdf)
            //            .Union(FormatDocumentExtensions.Tiff)));

        }
    }


}
