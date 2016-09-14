﻿using Autofac;
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
            Ioc.RegisterType<IJob, ThumbnailQueueProcess>(nameof(ThumbnailQueueProcess));
            Ioc.RegisterType<IJob, MailQueueProcess>(nameof(MailQueueProcess));
            Ioc.RegisterType<IJob, TestingJob>(nameof(TestingJob));

            Ioc.RegisterType<IJob, DeleteOldStuff>(nameof(DeleteOldStuff));
            Ioc.RegisterType<IJob, DeleteOldConnections>(nameof(DeleteOldConnections));


            Ioc.RegisterType<ISchedulerProcess, NoUniversityMailProcess>("universityNotSelected");
            Ioc.RegisterType<ISchedulerProcess, NoFollowClassMailProcess>("notFollowing");
            Ioc.RegisterType<ISchedulerProcess, UniversityWithLowActivation>("universityLowActivity");
            Ioc.RegisterType<ISchedulerProcess, FollowLowActivityCourses>("followLowActivity");
            Ioc.RegisterType<ISchedulerProcess, LikesMailProcess>("likesReport");

            var arrayOfUtcOfset = new[] { 0, 3, -5, -4 };
            foreach (var i in arrayOfUtcOfset)
            {
                Ioc.ContainerBuilder.RegisterType<DigestEmail>()
               .Named<ISchedulerProcess>($"digestOnceADay-{i}")
               .WithParameter("hourForEmailDigest", NotificationSettings.OnceADay)
               .WithParameter("utcTimeOffset", i);

                Ioc.ContainerBuilder.RegisterType<DigestEmail>()
                    .Named<ISchedulerProcess>($"digestOnceAWeek-{i}")
                    .WithParameter("hourForEmailDigest", NotificationSettings.OnceAWeek)
                    .WithParameter("utcTimeOffset", i);
            }



            Ioc.RegisterType<ISchedulerProcess, SpamGun>("spamGun");

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
            Ioc.RegisterType<IDomainProcess, NewUserProcess>(
                 Infrastructure.Transport.DomainProcess.UserResolver);

            Ioc.RegisterType<IFileProcess, PreProcessFile>(nameof(ChatFileProcessData));
            Ioc.RegisterType<IFileProcess, UpdateSearchItem>(nameof(BoxFileProcessData));
            Ioc.RegisterType<IFileProcess, UpdateSearchUniversity>(nameof(UniversityProcessData));

        }

        public T Resolve<T>(string name)
        {
            return Ioc.Resolve<T>(name);
        }
    }
}
