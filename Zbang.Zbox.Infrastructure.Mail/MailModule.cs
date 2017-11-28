using Autofac;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MailManager2>().As<IMailComponent>();

            builder.RegisterType<WelcomeMail>().Named<IMailBuilder>(MailParameters.WelcomeResolver);
            builder.RegisterType<InviteMail>().Named<IMailBuilder>(MailParameters.InviteResolver);
            builder.RegisterType<ForgotPasswordMail>().Named<IMailBuilder>(MailParameters.ForgotPswResolver);
            builder.RegisterType<MessageMail>().Named<IMailBuilder>(MailParameters.MessageResolver);
            builder.RegisterType<UpdatesMail>().Named<IMailBuilder>(MailParameters.UpdateResolver);
            builder.RegisterType<ChangeEmailMail>().Named<IMailBuilder>(MailParameters.ChangeEmailResolver);
            builder.RegisterType<FlagItemMail>().Named<IMailBuilder>(MailParameters.FlagBadItemResolver);
            builder.RegisterType<InvitationToCloudentsMail>().Named<IMailBuilder>(MailParameters.InvitationToCloudentsResolver);
            builder.RegisterType<DepartmentRequestAccessMail>().Named<IMailBuilder>(MailParameters.DepartmentRequestAccessResolver);
            builder.RegisterType<DepartmentApprovedAccessMail>().Named<IMailBuilder>(MailParameters.DepartmentRequestApprovedResolver);
            builder.RegisterType<NoUniversityMail>().Named<IMailBuilder>(nameof(NoUniversityMailParams));
            builder.RegisterType<NoFollowingBoxMail>().Named<IMailBuilder>(nameof(NoFollowingBoxMailParams));
            builder.RegisterType<UniversityLowActivityMail>().Named<IMailBuilder>(nameof(UniversityLowActivityMailParams));
            builder.RegisterType<LowCoursesActivityMail>().Named<IMailBuilder>(nameof(LowCoursesActivityMailParams));
            builder.RegisterType<LikesMail>().Named<IMailBuilder>(nameof(LikesMailParams));
            builder.RegisterType<LowContributionMail>().Named<IMailBuilder>(nameof(LowContributionMailParams));
            builder.RegisterType<ReplyToCommentMail>().Named<IMailBuilder>(nameof(ReplyToCommentMailParams));
            builder.RegisterType<SpamGunMail>().Named<IMailBuilder>(nameof(SpamGunMailParams));
           // builder.RegisterType<GreekPartnerMail>().Named<IMailBuilder>(nameof(GreekPartnerMailParams));
            builder.RegisterType<EmailVerification>().As<IEmailVerification>().SingleInstance();
        }
    }
}
