using Autofac;

namespace Zbang.Zbox.ReadServices
{
    public class ReadServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentDbReadService>().As<IDocumentDbReadService>();
            builder.RegisterType<ZboxReadServiceWorkerRole>().As<IZboxReadServiceWorkerRole>();
        }
    }
}
