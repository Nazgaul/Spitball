using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Infrastructure.Storage
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.IcoSite)]
    [ModuleRegistration(Core.Enum.System.Admin)]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Autofac module")]
    public class ModuleServiceBus : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var t = c.Resolve<IConfigurationKeys>();
                return new ServiceBusProvider(t.ServiceBus);
            }).As<IServiceBusProvider>();
            //builder.RegisterType<ServiceBusProvider>().As<IServiceBusProvider>()

            /*.As<IStartable>().SingleInstance().AutoActivate()*/
            ;
            base.Load(builder);
        }
    }
}