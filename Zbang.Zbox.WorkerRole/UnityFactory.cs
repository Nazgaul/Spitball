﻿using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.WorkerRole.DomainProcess;
using Zbang.Zbox.WorkerRole.Jobs;
using Zbang.Zbox.WorkerRole.Mail;
using Zbang.Zbox.WorkerRole.OneTimeUpdates;

namespace Zbang.Zbox.WorkerRole
{
    internal class UnityFactory
    {
        // UnityContainer unityFactory;
        public const string DeleteCahceBlobContainer = "deleteCacheBlobContainer";
        public const string GenerateDocumentCache = "generateDocumentCache";
        public const string MailProcess = "mailProcess";
        public const string MailProcess2 = "mailProcess2";
        public const string ThumbnailProcess = "thumbnailProcess";
        public const string DigestEmail = "digestEmail";
        public const string DigestEmail2 = "digestEmail2";
        public const string AddFiles = "addFiles";

        public const string Dbi = "Dbi";
        public const string Transaction = "Transaction";

        public Infrastructure.Ioc.IocFactory Unity { get; private set; }
        public UnityFactory()
        {
            Unity = Infrastructure.Ioc.IocFactory.Unity;


            Infrastructure.RegisterIoc.Register();
            Infrastructure.Data.RegisterIoc.Register();
            Infrastructure.File.RegisterIoc.Register();
            Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Infrastructure.Mail.RegisterIoc.Register();
            Domain.Services.RegisterIoc.Register();
            ReadServices.RegisterIoc.Register();
            Domain.CommandHandlers.Ioc.RegisterIoc.Register();

           

            //Unity = new UnityContainer();
            RegisterTypes();
            Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
        }

        private void RegisterTypes()
        {
            Unity.RegisterType<IJob, DeleteCacheBlobContainer>(DeleteCahceBlobContainer);
            Unity.RegisterType<IJob, ProcessFile>(GenerateDocumentCache);
            Unity.RegisterType<IJob, DigestEmail2>(DigestEmail2);
            Unity.RegisterType<IJob, UpdateDataBase>(Dbi);
            Unity.RegisterType<IJob, UpdateDomainProcess>(Transaction);
            Unity.RegisterType<IJob, MailProcess2>(MailProcess2);
            Unity.RegisterType<IJob, AddFiles>(AddFiles);

            Unity.RegisterType<Imail2, Welcome>(BaseMailData.WelcomeResolver);
            Unity.RegisterType<Imail2, Invite2>(BaseMailData.InviteResolver);
            Unity.RegisterType<Imail2, ForgotPassword>(BaseMailData.ForgotPasswordResolver);
            Unity.RegisterType<Imail2, Message2>(BaseMailData.MessageResolver);
            Unity.RegisterType<Imail2, ChangeEmail>(BaseMailData.ChangeEmailResolver);
            Unity.RegisterType<Imail2, InviteToCloudents>(BaseMailData.InviteToCloudentsResolver);


            //Unity.RegisterType<IDomainProcess, AddFriend>(Zbang.Zbox.Infrastructure.Transport.DomainProcess.AddAFriendResolver);
            Unity.RegisterType<IDomainProcess, Statistics>(Zbang.Zbox.Infrastructure.Transport.DomainProcess.StatisticsResolver);
            Unity.RegisterType<IDomainProcess, FlagBadItem>(Zbang.Zbox.Infrastructure.Transport.DomainProcess.BadItemResolver);
            Unity.RegisterType<IDomainProcess, UpdatesProcess>(Zbang.Zbox.Infrastructure.Transport.DomainProcess.UpdateResolver);


            Unity.RegisterType<IUpdateThumbnails, UpdateThumbnails>();

        }
       
        public T Resolve<T>(string name)
        {
            return Unity.Resolve<T>(name);
        }
    }
}
