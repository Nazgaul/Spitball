using Autofac;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ReadServices
{
    public class ReadServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentDbReadService>().As<IDocumentDbReadService>();
            builder.RegisterType<ZboxReadService>().As<IZboxReadService>().As< IUniversityWithCode>();
            builder.RegisterType<ZboxReadServiceWorkerRole>().As<IZboxReadServiceWorkerRole>();
        }
    }
}
