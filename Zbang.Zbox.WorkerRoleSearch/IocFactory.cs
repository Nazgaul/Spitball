﻿using Autofac;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
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
        // UnityContainer unityFactory;
        //public const string DeleteCacheBlobContainer = "deleteCacheBlobContainer";
        //public const string PreProcessFiles = "generateDocumentCache";
        //public const string MailProcess2 = "mailProcess2";
        //public const string DigestEmail2 = "digestEmail2";
        //public const string AddFiles = "addFiles";
        //
        //public const string Dbi = "Dbi";
        //public const string Transaction = "Transaction";
        //
        //public const string Product = "Product";
        //public const string StoreOrder = "StoreOrder";
        //
        //public const string EmailPartners = "EmailPartners";

        public const string UpdateSearchItem = "UpdateSearchItem";
        public const string UpdateSearchBox = "UpdateSearchBox";
        public const string UpdateSearchQuiz = "UpdateSearchQuiz";
        public const string UpdateSearchUniversity = "UpdateSearchUniversity";

        public Infrastructure.Ioc.IocFactory Unity { get; }
        public IocFactory()
        {
            Unity = Infrastructure.Ioc.IocFactory.IocWrapper;


            RegisterIoc.Register();
            Infrastructure.Data.RegisterIoc.Register();
            Infrastructure.File.RegisterIoc.Register();
            Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Infrastructure.Mail.RegisterIoc.Register();

            Unity.ContainerBuilder.RegisterType<SeachConnection>()
             .As<ISearchConnection>()
             .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
             .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
             .WithParameter("isDevelop", false)
             .InstancePerLifetimeScope();
            Infrastructure.Search.RegisterIoc.Register();
            //Infrastructure.Mail.RegisterIoc.Register();
            Domain.Services.RegisterIoc.Register();
            ReadServices.RegisterIoc.Register();
            Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            //Store.RegisterIoc.Register();


            //Unity = new UnityContainer();
            RegisterTypes();
            Unity.Build();
            Unity.Resolve<IBlobProvider>();
        }

        private void RegisterTypes()
        {
            //Unity.RegisterType<IJob, DeleteCacheBlobContainer>(DeleteCahceBlobContainer);
            //Unity.RegisterType<IJob, ProcessFile>(PreProcessFiles);
            
            //Unity.RegisterType<IJob, UpdateDataBase>(Dbi);
            //Unity.RegisterType<IJob, UpdateDomainProcess>(Transaction);
            //Unity.RegisterType<IJob, MailProcess2>(MailProcess2);
            //Unity.RegisterType<IJob, AddFiles>(AddFiles);
            //Unity.RegisterType<IJob, PartnersEmail>(EmailPartners);
            //Unity.RegisterType<IJob, StoreDataSync>(Product);
            //Unity.RegisterType<IJob, ProcessStoreOrder>(StoreOrder);
            Unity.RegisterType<IJob, UpdateSearchItem>(UpdateSearchItem);
            Unity.RegisterType<IJob, UpdateSearchBox>(UpdateSearchBox);
            Unity.RegisterType<IJob, UpdateSearchQuiz>(UpdateSearchQuiz);
            Unity.RegisterType<IJob, UpdateSearchUniversity>(UpdateSearchUniversity);
            Unity.RegisterType<IJob, SchedulerListener>(nameof(SchedulerListener));
            Unity.RegisterType<IJob, UpdateUnsubscribeList>(nameof(UpdateUnsubscribeList));
            Unity.RegisterType<IJob, TransactionQueueProcess>(nameof(TransactionQueueProcess));
            Unity.RegisterType<IJob, MailQueueProcess>(nameof(MailQueueProcess));
            Unity.RegisterType<IJob, TestingJob>(nameof(TestingJob));
            Unity.RegisterType<IMailProcess, NoUniversityMailProcess>("universityNotSelected");
            Unity.RegisterType<IMailProcess, NoFollowClassMailProcess>("notFollowing");
            Unity.RegisterType<IMailProcess, UniversityWithLowActivation>("universityLowActivity");
            Unity.RegisterType<IMailProcess, FollowLowActivityCourses>("followLowActivity");
            Unity.RegisterType<IMailProcess, LikesMailProcess>("likesReport");


            Unity.ContainerBuilder.RegisterType<DigestEmail>()
                .Named<IMailProcess>("digestOnceADay")
                .WithParameter("hourForEmailDigest", NotificationSettings.OnceADay);

            Unity.ContainerBuilder.RegisterType<DigestEmail>()
                .Named<IMailProcess>("digestOnceAWeek")
                .WithParameter("hourForEmailDigest", NotificationSettings.OnceAWeek);

            Unity.ContainerBuilder.RegisterType<DigestEmail>()
                .Named<IMailProcess>("digestEveryChange")
                .WithParameter("hourForEmailDigest", NotificationSettings.OnEveryChange);



            Unity.RegisterType<IMail2, Welcome>(BaseMailData.WelcomeResolver);
            Unity.RegisterType<IMail2, Invite2>(BaseMailData.InviteResolver);
            Unity.RegisterType<IMail2, ForgotPassword>(BaseMailData.ForgotPasswordResolver);
            Unity.RegisterType<IMail2, Message2>(BaseMailData.MessageResolver);
            Unity.RegisterType<IMail2, ChangeEmail>(BaseMailData.ChangeEmailResolver);
            Unity.RegisterType<IMail2, InviteToCloudents>(BaseMailData.InviteToCloudentsResolver);
            Unity.RegisterType<IMail2, RequestAccess>(BaseMailData.RequestAccessResolver);
            Unity.RegisterType<IMail2, LibraryAccessApproved>(BaseMailData.AccessApprovedResolver);
            Unity.RegisterType<IMail2, ReplyToComment>(nameof(ReplyToCommentData));


            Unity.RegisterType<IDomainProcess, Statistics>(Infrastructure.Transport.DomainProcess.StatisticsResolver);
            Unity.RegisterType<IDomainProcess, FlagBadItem>(Infrastructure.Transport.DomainProcess.BadItemResolver);
            Unity.RegisterType<IDomainProcess, UpdatesProcess>(Infrastructure.Transport.DomainProcess.UpdateResolver);
            //Unity.RegisterType<IDomainProcess, UpdateUniversitySearch>(Infrastructure.Transport.DomainProcess.UniversityResolver);
            Unity.RegisterType<IDomainProcess, UpdateReputation>(
                Infrastructure.Transport.DomainProcess.ReputationResolver);
            Unity.RegisterType<IDomainProcess, UpdateQuota>(
                Infrastructure.Transport.DomainProcess.QuotaResolver);
            Unity.RegisterType<IDomainProcess, DeleteBox>(
                Infrastructure.Transport.DomainProcess.DeleteBoxResolver);
            //Unity.RegisterType<IDomainProcess, Statistics>(Infrastructure.Transport.DomainProcess.StatisticsResolver);
            //Unity.RegisterType<IDomainProcess, FlagBadItem>(Infrastructure.Transport.DomainProcess.BadItemResolver);
            //Unity.RegisterType<IDomainProcess, UpdatesProcess>(Infrastructure.Transport.DomainProcess.UpdateResolver);
            ////Unity.RegisterType<IDomainProcess, UpdateUniversitySearch>(Infrastructure.Transport.DomainProcess.UniversityResolver);
            //Unity.RegisterType<IDomainProcess, UpdateReputation>(
            //    Infrastructure.Transport.DomainProcess.ReputationResolver);
            //Unity.RegisterType<IDomainProcess, UpdateQuota>(
            //    Infrastructure.Transport.DomainProcess.QuotaResolver);

        }

        public T Resolve<T>(string name)
        {
            return Unity.Resolve<T>(name);
        }
    }
}
