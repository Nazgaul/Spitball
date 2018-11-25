using Autofac;
using CacheManager.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Cache;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    [ModuleRegistration(Core.Enum.System.Admin)]
    [ModuleRegistration(Core.Enum.System.Function)]

    public class ModuleCache : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<CacheProvider>().AsSelf().SingleInstance().AsImplementedInterfaces();
        }
    }
}