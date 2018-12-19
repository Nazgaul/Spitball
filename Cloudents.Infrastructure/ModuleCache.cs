using Autofac;
using Cloudents.Application.Attributes;
using Cloudents.Infrastructure.Cache;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.WorkerRole)]
    [ModuleRegistration(Application.Enum.System.Admin)]
    [ModuleRegistration(Application.Enum.System.Function)]

    public class ModuleCache : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<CacheProvider>().AsSelf().SingleInstance().AsImplementedInterfaces();
        }
    }
}