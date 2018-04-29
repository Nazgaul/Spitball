using Autofac;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WorkerRoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpdateSearchItem>().Named<IJob>(IocFactory.UpdateSearchItem);//
            builder.RegisterType<UpdateSearchBox>().Named<IJob>(IocFactory.UpdateSearchBox);//
            builder.RegisterType<UpdateSearchQuiz>().Named<IJob>(IocFactory.UpdateSearchQuiz);//
            builder.RegisterType<UpdateSearchFlashcard>().Named<IJob>(IocFactory.UpdateSearchFlashcard);//
            builder.RegisterType<UpdateSearchUniversity>().Named<IJob>(IocFactory.UpdateSearchUniversity);//

            builder.RegisterType<SchedulerListener>().Named<IJob>(nameof(SchedulerListener));//
            builder.RegisterType<UpdateUnsubscribeList>().Named<IJob>(nameof(UpdateUnsubscribeList));//
            builder.RegisterType<TransactionQueueProcess>().Named<IJob>(nameof(TransactionQueueProcess));//
            builder.RegisterType<ThumbnailQueueProcess>().Named<IJob>(nameof(ThumbnailQueueProcess));//
            builder.RegisterType<MailQueueProcess>().Named<IJob>(nameof(MailQueueProcess));//
            builder.RegisterType<TestingJob>().Named<IJob>(nameof(TestingJob));
            builder.RegisterType<DeleteOldConnections>().Named<IJob>(nameof(DeleteOldConnections));//

            builder.RegisterType<NoUniversityMailProcess>().Keyed<ISchedulerProcess>("universityNotSelected");
            builder.RegisterType<NoFollowClassMailProcess>().Keyed<ISchedulerProcess>("notFollowing");
            builder.RegisterType<UniversityWithLowActivation>().Keyed<ISchedulerProcess>("universityLowActivity");
            builder.RegisterType<FollowLowActivityCourses>().Keyed<ISchedulerProcess>("followLowActivity");
            builder.RegisterType<LikesMailProcess>().Keyed<ISchedulerProcess>("likesReport");

            builder.RegisterType<DeleteOldStuff>().Keyed<ISchedulerProcess>("deleteOld");
            //builder.Register(c=>
            //{
            //    var affiliate = c.ResolveKeyed<IUpdateAffiliate>(AffiliateProgram.CareerBuilder);
            //    return new AffiliateBuilder(affiliate);
            //}).Keyed<ISchedulerProcess>("careerBuilder");

            builder.Register(c =>
            {
                var affiliate = c.ResolveKeyed<IUpdateAffiliate>(AffiliateProgram.Wyzant);
                return new AffiliateBuilder(affiliate);
            }).Keyed<ISchedulerProcess>("downloadTutor");

            builder.Register(c =>
            {
                var affiliate = c.ResolveKeyed<IUpdateAffiliate>(AffiliateProgram.WayUp);
                return new AffiliateBuilder(affiliate);
            }).Keyed<ISchedulerProcess>("downloadXml");

            foreach (var utcTimeOffset in new[] { 0, 3/*, -5, -4, -6*/ })
            {
                builder.RegisterType<DigestEmail>()
                    .Keyed<ISchedulerProcess>($"digestOnceADay_{utcTimeOffset}")
                    .WithParameter("hourForEmailDigest", NotificationSetting.OnceADay)
                    .WithParameter("utcTimeOffset", utcTimeOffset);

                builder.RegisterType<DigestEmail>()
                    .Keyed<ISchedulerProcess>($"digestOnceAWeek_{utcTimeOffset}")
                    .WithParameter("hourForEmailDigest", NotificationSetting.OnceAWeek)
                    .WithParameter("utcTimeOffset", utcTimeOffset);
            }

            builder.RegisterType<SpamGun>().Keyed<ISchedulerProcess>("spamGun");

            builder.RegisterType<IntercomApiManager>().As<IIntercomApiManager>();

            builder.RegisterType<Welcome>().Named<IMail2>(BaseMailData.WelcomeResolver);
            builder.RegisterType<Invite2>().Named<IMail2>(BaseMailData.InviteResolver);
            builder.RegisterType<ForgotPassword>().Named<IMail2>(BaseMailData.ForgotPasswordResolver);
            builder.RegisterType<Message2>().Named<IMail2>(BaseMailData.MessageResolver);
            builder.RegisterType<ChangeEmail>().Named<IMail2>(BaseMailData.ChangeEmailResolver);
            builder.RegisterType<InviteToCloudents>().Named<IMail2>(BaseMailData.InviteToCloudentsResolver);
            builder.RegisterType<RequestAccess>().Named<IMail2>(BaseMailData.RequestAccessResolver);
            builder.RegisterType<LibraryAccessApproved>().Named<IMail2>(BaseMailData.AccessApprovedResolver);
            builder.RegisterType<ReplyToComment>().Named<IMail2>(nameof(ReplyToCommentData));

            builder.RegisterType<Statistics>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.StatisticsResolver);
            builder.RegisterType<UpdatesProcess>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.UpdateResolver);
            builder.RegisterType<UpdateReputation>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.ReputationResolver);
            builder.RegisterType<UpdateBadge>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.BadgeResolver);
            builder.RegisterType<UpdateQuota>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.QuotaResolver);
            builder.RegisterType<DeleteBox>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.DeleteBoxResolver);
            builder.RegisterType<NewUserProcess>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.UserResolver);

            builder.RegisterType<PreProcessFile>().Named<IFileProcess>(nameof(ChatFileProcessData));
            builder.RegisterType<UpdateSearchItem>().Named<IFileProcess>(nameof(BoxFileProcessData));
            builder.RegisterType<UpdateSearchUniversity>().Named<IFileProcess>(nameof(UniversityProcessData));
            builder.RegisterType<UpdateSearchBox>().Named<IFileProcess>(nameof(BoxProcessData));

            builder.RegisterType<TelemetryLogger>().As<ILogger>();
            base.Load(builder);
        }
    }
}