using Autofac;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;
using Zbang.Zbox.WorkerRoleSearch.Mail;
using RegisterIoc = Zbang.Zbox.Infrastructure.RegisterIoc;

namespace Zbang.Zbox.WorkerRoleSearch
{
    internal class IocFactory
    {
        

        public const string UpdateSearchItem = "UpdateSearchItem";
        public const string UpdateSearchBox = "UpdateSearchBox";
        public const string UpdateSearchQuiz = "UpdateSearchQuiz";
        public const string UpdateSearchUniversity = "UpdateSearchUniversity";

        public Infrastructure.Ioc.IocFactory Ioc { get; }
        public IocFactory()
        {
            Ioc = Infrastructure.Ioc.IocFactory.IocWrapper;


            RegisterIoc.Register();
            Infrastructure.Data.RegisterIoc.Register();
            Infrastructure.File.RegisterIoc.Register();
            Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Infrastructure.Mail.RegisterIoc.Register();

            Ioc.ContainerBuilder.RegisterType<SeachConnection>()
             .As<ISearchConnection>()
             .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
             .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
             .WithParameter("isDevelop", false)
             .InstancePerLifetimeScope();
            Infrastructure.Search.RegisterIoc.Register();
            Domain.Services.RegisterIoc.Register();
            ReadServices.RegisterIoc.Register();
            Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            //Store.RegisterIoc.Register();
            Ioc.ContainerBuilder.RegisterType<SendPush>()
            .As<ISendPush>()
            .WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
            .WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
            .InstancePerLifetimeScope();

            //Unity = new UnityContainer();
            RegisterTypes();
            Ioc.Build();
            Ioc.Resolve<IBlobProvider>();
        }

        private void RegisterTypes()
        {
            Ioc.RegisterType<IJob, UpdateSearchItem>(UpdateSearchItem);
            Ioc.RegisterType<IJob, UpdateSearchBox>(UpdateSearchBox);
            Ioc.RegisterType<IJob, UpdateSearchQuiz>(UpdateSearchQuiz);
            Ioc.RegisterType<IJob, UpdateSearchUniversity>(UpdateSearchUniversity);
            Ioc.RegisterType<IJob, SchedulerListener>(nameof(SchedulerListener));
            Ioc.RegisterType<IJob, UpdateUnsubscribeList>(nameof(UpdateUnsubscribeList));
            Ioc.RegisterType<IJob, TransactionQueueProcess>(nameof(TransactionQueueProcess));
            Ioc.RegisterType<IJob, MailQueueProcess>(nameof(MailQueueProcess));
            Ioc.RegisterType<IJob, TestingJob>(nameof(TestingJob));
            Ioc.RegisterType<IMailProcess, NoUniversityMailProcess>("universityNotSelected");
            Ioc.RegisterType<IMailProcess, NoFollowClassMailProcess>("notFollowing");
            Ioc.RegisterType<IMailProcess, UniversityWithLowActivation>("universityLowActivity");
            Ioc.RegisterType<IMailProcess, FollowLowActivityCourses>("followLowActivity");
            Ioc.RegisterType<IMailProcess, LikesMailProcess>("likesReport");


            Ioc.ContainerBuilder.RegisterType<DigestEmail>()
                .Named<IMailProcess>("digestOnceADay")
                .WithParameter("hourForEmailDigest", NotificationSettings.OnceADay);

            Ioc.ContainerBuilder.RegisterType<DigestEmail>()
                .Named<IMailProcess>("digestOnceAWeek")
                .WithParameter("hourForEmailDigest", NotificationSettings.OnceAWeek);

            Ioc.ContainerBuilder.RegisterType<DigestEmail>()
                .Named<IMailProcess>("digestEveryChange")
                .WithParameter("hourForEmailDigest", NotificationSettings.OnEveryChange);

            Ioc.RegisterType<IIntercomApiManager, IntercomApiManager>();

            Ioc.RegisterType<IMail2, Welcome>(BaseMailData.WelcomeResolver);
            Ioc.RegisterType<IMail2, Invite2>(BaseMailData.InviteResolver);
            Ioc.RegisterType<IMail2, ForgotPassword>(BaseMailData.ForgotPasswordResolver);
            Ioc.RegisterType<IMail2, Message2>(BaseMailData.MessageResolver);
            Ioc.RegisterType<IMail2, ChangeEmail>(BaseMailData.ChangeEmailResolver);
            Ioc.RegisterType<IMail2, InviteToCloudents>(BaseMailData.InviteToCloudentsResolver);
            Ioc.RegisterType<IMail2, RequestAccess>(BaseMailData.RequestAccessResolver);
            Ioc.RegisterType<IMail2, LibraryAccessApproved>(BaseMailData.AccessApprovedResolver);
            Ioc.RegisterType<IMail2, ReplyToComment>(nameof(ReplyToCommentData));


            Ioc.RegisterType<IDomainProcess, Statistics>(Infrastructure.Transport.DomainProcess.StatisticsResolver);
            Ioc.RegisterType<IDomainProcess, FlagBadItem>(Infrastructure.Transport.DomainProcess.BadItemResolver);
            Ioc.RegisterType<IDomainProcess, UpdatesProcess>(Infrastructure.Transport.DomainProcess.UpdateResolver);
            Ioc.RegisterType<IDomainProcess, UpdateReputation>(
                Infrastructure.Transport.DomainProcess.ReputationResolver);
            Ioc.RegisterType<IDomainProcess, UpdateQuota>(
                Infrastructure.Transport.DomainProcess.QuotaResolver);
            Ioc.RegisterType<IDomainProcess, DeleteBox>(
                Infrastructure.Transport.DomainProcess.DeleteBoxResolver);
            
        }

        public T Resolve<T>(string name)
        {
            return Ioc.Resolve<T>(name);
        }
    }
}
