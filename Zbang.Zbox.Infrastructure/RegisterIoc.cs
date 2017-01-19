using Autofac;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure
{
    //public static class RegisterIoc
    //{
    //    public static void Register()
    //    {
    //        var ioc = IocFactory.IocWrapper;

    //        ioc.RegisterType<Cache.ICache, Cache.SystemCache>(LifeTimeManager.Singleton);
    //        ioc.RegisterType<Cache.IWithCache, Cache.WithCache>(LifeTimeManager.Singleton);
    //        ioc.RegisterType<Url.IEncryptObject, Url.EncryptObject>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<CommandHandlers.ICommandBus, CommandHandlers.CommandBus>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<Profile.IProfilePictureProvider, Profile.ProfilePictureProvider>(LifeTimeManager.PerHttpRequest);

    //        ioc.RegisterType<Security.IFacebookService, Security.FacebookAuthenticationService>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<Security.IGoogleService, Security.GoogleAuthenticationService>(LifeTimeManager.PerHttpRequest);

    //        ioc.RegisterType<IdGenerator.IGuidIdGenerator, IdGenerator.GuidIdGenerator>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IDetectLanguage, DetectLanguage>(LifeTimeManager.Singleton);
    //        ioc.RegisterType<ILocationProvider, LocationProvider>(LifeTimeManager.PerHttpRequest);
    //    }
    //}

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
            builder.RegisterType<DetectLanguage>().As<IDetectLanguage>().SingleInstance();
            builder.RegisterType<LocationProvider>().As<ILocationProvider>();
        }
    }
}
