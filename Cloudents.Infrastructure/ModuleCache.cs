using Autofac;
using Cloudents.Infrastructure.Cache;

namespace Cloudents.Infrastructure
{
    public class ModuleCache : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<CacheProvider>().AsSelf().SingleInstance().AsImplementedInterfaces();
        }
    }
}