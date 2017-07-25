using Autofac;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WorkerRoleModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpdateSearchItem>().Named<IJob>(IocFactory.UpdateSearchItem);
            builder.RegisterType<UpdateSearchBox>().Named<IJob>(IocFactory.UpdateSearchBox);
            builder.RegisterType<UpdateSearchQuiz>().Named<IJob>(IocFactory.UpdateSearchQuiz);
            builder.RegisterType<UpdateSearchFlashcard>().Named<IJob>(IocFactory.UpdateSearchFlashcard);
            builder.RegisterType<UpdateSearchUniversity>().Named<IJob>(IocFactory.UpdateSearchUniversity);
            builder.RegisterType<UpdateSearchFeed>().Named<IJob>(nameof(UpdateSearchFeed));
            builder.RegisterType<Crawler>().Named<IJob>(nameof(Crawler));



            builder.RegisterType<SchedulerListener>().Named<IJob>(nameof(SchedulerListener));
            builder.RegisterType<UpdateUnsubscribeList>().Named<IJob>(nameof(UpdateUnsubscribeList));
            builder.RegisterType<TransactionQueueProcess>().Named<IJob>(nameof(TransactionQueueProcess));
            builder.RegisterType<ThumbnailQueueProcess>().Named<IJob>(nameof(ThumbnailQueueProcess));
            builder.RegisterType<MailQueueProcess>().Named<IJob>(nameof(MailQueueProcess));
            builder.RegisterType<TestingJob>().Named<IJob>(nameof(TestingJob));
            builder.RegisterType<BlobManagement>().Named<IJob>(nameof(BlobManagement));
            builder.RegisterType<DeleteOldConnections>().Named<IJob>(nameof(DeleteOldConnections));

            builder.RegisterType<NoUniversityMailProcess>().Named<ISchedulerProcess>("universityNotSelected");
            builder.RegisterType<NoFollowClassMailProcess>().Named<ISchedulerProcess>("notFollowing");
            builder.RegisterType<UniversityWithLowActivation>().Named<ISchedulerProcess>("universityLowActivity");
            builder.RegisterType<FollowLowActivityCourses>().Named<ISchedulerProcess>("followLowActivity");
            builder.RegisterType<LikesMailProcess>().Named<ISchedulerProcess>("likesReport");

            builder.RegisterType<DeleteOldStuff>().Named<ISchedulerProcess>("deleteOld");
            builder.RegisterType<UpdateJobs>().Named<ISchedulerProcess>("downloadXml");

            var arrayOfUtcOffset = new[] { 0, 3, -5, -4, -6 };
            foreach (var i in arrayOfUtcOffset)
            {
                builder.RegisterType<DigestEmail>()
                    .Named<ISchedulerProcess>($"digestOnceADay_{i}")
                    .WithParameter("hourForEmailDigest", NotificationSetting.OnceADay)
                    .WithParameter("utcTimeOffset", i);

                builder.RegisterType<DigestEmail>()
                    .Named<ISchedulerProcess>($"digestOnceAWeek_{i}")
                    .WithParameter("hourForEmailDigest", NotificationSetting.OnceAWeek)
                    .WithParameter("utcTimeOffset", i);
            }

            builder.RegisterType<SpamGun>().Named<ISchedulerProcess>("spamGun");

            builder.RegisterType<IntercomApiManager>().As<IIntercomApiManager>();
            builder.RegisterType<WatsonExtract>().As<IWatsonExtract>();

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
            builder.RegisterType<FlagBadItem>().Named<IDomainProcess>(Infrastructure.Transport.DomainProcess.BadItemResolver);
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