using Autofac;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRole.DomainProcess;
using Zbang.Zbox.WorkerRole.Jobs;
using Zbang.Zbox.WorkerRole.Mail;

namespace Zbang.Zbox.WorkerRole
{
    internal class UnityFactory
    {
        // UnityContainer unityFactory;
        public const string DeleteCacheBlobContainer = "deleteCacheBlobContainer";
        public const string PreProcessFiles = "generateDocumentCache";
        public const string MailProcess2 = "mailProcess2";
        public const string DigestEmail2 = "digestEmail2";
        public const string AddFiles = "addFiles";

        public const string Dbi = "Dbi";
        public const string Transaction = "Transaction";

        //public const string Product = "Product";
       // public const string StoreOrder = "StoreOrder";

        //public const string EmailPartners = "EmailPartners";
        public const string UpdateSearch = "UpdateSearch";

        public IocFactory Unity { get; private set; }
        public UnityFactory()
        {
            Unity = IocFactory.IocWrapper;


            RegisterIoc.Register();
            Infrastructure.Data.RegisterIoc.Register();
            Infrastructure.File.RegisterIoc.Register();
            Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Infrastructure.Mail.RegisterIoc.Register();
            Domain.Services.RegisterIoc.Register();
            ReadServices.RegisterIoc.Register();
            Domain.CommandHandlers.Ioc.RegisterIoc.Register();

           // Store.RegisterIoc.Register();

            Unity.ContainerBuilder.RegisterType<SendPush>()
             .As<ISendPush>()
             .WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
             .WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
             .InstancePerLifetimeScope();

            //Unity = new UnityContainer();
            RegisterTypes();
            Unity.Build();
            Unity.Resolve<IBlobProvider>();
        }

        private void RegisterTypes()
        {
            //Unity.RegisterType<IJob, DeleteCacheBlobContainer>(DeleteCahceBlobContainer);
            //Unity.RegisterType<IJob, ProcessFile>(PreProcessFiles);
            Unity.RegisterType<IJob, DigestEmail2>(DigestEmail2);
            Unity.RegisterType<IJob, UpdateDataBase>(Dbi);
            Unity.RegisterType<IJob, UpdateDomainProcess>(Transaction);
            Unity.RegisterType<IJob, MailProcess2>(MailProcess2);
            Unity.RegisterType<IJob, AddFiles>(AddFiles);
            //Unity.RegisterType<IJob, PartnersEmail>(EmailPartners);
           // Unity.RegisterType<IJob, StoreDataSync>(Product);
           // Unity.RegisterType<IJob, ProcessStoreOrder>(StoreOrder);
            //Unity.RegisterType<IJob, UpdateSearch>(UpdateSearch);

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
            Unity.RegisterType<IDomainProcess, UpdateReputation>(
                Infrastructure.Transport.DomainProcess.ReputationResolver);
            Unity.RegisterType<IDomainProcess, UpdateQuota>(
                Infrastructure.Transport.DomainProcess.QuotaResolver);
            Unity.RegisterType<IDomainProcess, DeleteBox>(
                Infrastructure.Transport.DomainProcess.DeleteBoxResolver);

        }
       
        public T Resolve<T>(string name)
        {
            return Unity.Resolve<T>(name);
        }
    }
}
