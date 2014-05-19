using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;


            ioc.RegisterType<Cache.ICache, Cache.Cache>(LifeTimeManager.Singleton);
            ioc.RegisterType<Cache.IWithCache, Cache.WithCache>(LifeTimeManager.Singleton);
            ioc.RegisterType<Cache.IHttpContextCacheWrapper, Cache.HttpContextCacheWrapper>(LifeTimeManager.PerHttpRequest);

            ioc.RegisterType<Url.IShortCodesCache, Url.ShortCodesCache>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<Url.IEncryptObject, Url.EncryptObject>(LifeTimeManager.PerHttpRequest);

            ioc.RegisterType<Url.IInviteLinkGenerator, Url.InviteLinkProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<Url.IInviteLinkDecrypt, Url.InviteLinkProvider>(LifeTimeManager.PerHttpRequest);

            //ioc.RegisterType<WebWorkerRoleJoinData.FileConvert.IFileConvertFactory,
            //    WebWorkerRoleJoinData.FileConvert.FileConvertFactory>(LifeTimeManager.PerHttpRequest);

            ioc.RegisterType<CommandHandlers.ICommandBus, CommandHandlers.CommandBus>(LifeTimeManager.PerHttpRequest);


            ioc.RegisterType<Thumbnail.IThumbnailProvider, Thumbnail.ThumbnailProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<Thumbnail.IAcademicBoxThumbnailProvider, Thumbnail.ThumbnailProvider>(LifeTimeManager.Singleton);



            ioc.RegisterType<Profile.IProfilePictureProvider, Profile.ProfilePictureProvider>(LifeTimeManager.PerHttpRequest);



            ioc.RegisterType<Security.IMembershipService, Security.AccountMembershipService>(LifeTimeManager.PerHttpRequest);

            ioc.RegisterType<Security.IFacebookAuthenticationService, Security.FacebookAuthenticationService>(LifeTimeManager.PerHttpRequest);

            ioc.RegisterType<Security.IFormsAuthenticationService, Security.FormsAuthenticationService>();

            ioc.RegisterType<Culture.IEnglishToHebrewChars, Culture.EnglishToHebrewChars>();
            ioc.RegisterType<Culture.IFilterWords, Culture.FilterWords>();
            ioc.RegisterType<Culture.IHebrewStemmer, Culture.HebrewStemmer>();


            ioc.RegisterType<IdGenerator.IIdGenerator, IdGenerator.IdGenerator>(LifeTimeManager.Singleton);

        }
    }
}
