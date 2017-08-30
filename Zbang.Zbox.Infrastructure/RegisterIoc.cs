using Autofac;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Cache.SystemCache>().As<Cache.ICache>().SingleInstance();
            builder.RegisterType<Cache.WithCache>().As<Cache.IWithCache>();
            builder.RegisterType<Url.EncryptObject>().As<Url.IEncryptObject>();
            builder.RegisterType<CommandHandlers.CommandBus>().As<CommandHandlers.ICommandBus>();
            builder.RegisterType<Profile.ProfilePictureProvider>().As<Profile.IProfilePictureProvider>();
            builder.RegisterType<Security.FacebookAuthenticationService>().As<Security.IFacebookService>();
            builder.RegisterType<Security.GoogleAuthenticationService>().As<Security.IGoogleService>();
            builder.RegisterType<IdGenerator.GuidIdGenerator>().As<IdGenerator.IGuidIdGenerator>();
            builder.RegisterType<LocationProvider>().As<ILocationProvider>();
            builder.RegisterType<Logger>().As<ILogger>();
        }
    }
}
