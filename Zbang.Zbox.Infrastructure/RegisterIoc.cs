using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;

            ioc.RegisterType<Cache.ICache, Cache.SystemCache>(LifeTimeManager.Singleton);
            ioc.RegisterType<Cache.IWithCache, Cache.WithCache>(LifeTimeManager.Singleton);
            ioc.RegisterType<Url.IShortCodesCache, Url.ShortCodesCache>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<Url.IEncryptObject, Url.EncryptObject>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<Url.IInviteLinkGenerator, Url.InviteLinkProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<Url.IInviteLinkDecrypt, Url.InviteLinkProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<CommandHandlers.ICommandBus, CommandHandlers.CommandBus>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<Profile.IProfilePictureProvider, Profile.ProfilePictureProvider>(LifeTimeManager.PerHttpRequest);

            ioc.RegisterType<Security.IFacebookService, Security.FacebookAuthenticationService>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<Security.IGoogleService, Security.GoogleAuthenticationService>(LifeTimeManager.PerHttpRequest);

            ioc.RegisterType<IdGenerator.IGuidIdGenerator, IdGenerator.GuidIdGenerator>(LifeTimeManager.Singleton);
        }
    }
}
