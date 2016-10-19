using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;

            ioc.RegisterType<IMailComponent, MailManager2>();
            ioc.RegisterType<IMailBuilder, WelcomeMail>(MailParameters.WelcomeResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, InviteMail>(MailParameters.InviteResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, ForgotPasswordMail>(MailParameters.ForgotPswResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, MessageMail>(MailParameters.MessageResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, UpdatesMail>(MailParameters.UpdateResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, ChangeEmailMail>(MailParameters.ChangeEmailResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, FlagItemMail>(MailParameters.FlagBadItemResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, InvitationToCloudentsMail>(MailParameters.InvitationToCloudentsResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, DepartmentRequestAccessMail>(MailParameters.DepartmentRequestAccessResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, DepartmentApprovedAccessMail>(MailParameters.DepartmentRequestApprovedResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, NoUniversityMail>(nameof(NoUniversityMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, NoFollowingBoxMail>(nameof(NoFollowingBoxMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, UniversityLowActivityMail>(nameof(UniversityLowActivityMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, LowCoursesActivityMail>(nameof(LowCoursesActivityMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, LikesMail>(nameof(LikesMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, LowContributionMail>(nameof(LowContributionMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, ReplyToCommentMail>(nameof(ReplyToCommentMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, SpamGunMail>(nameof(SpamGunMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, GreekPartnerMail>(nameof(GreekPartnerMailParams), LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IEmailVerification, EmailVerification>(LifeTimeManager.Singleton);

        }
    }
}
