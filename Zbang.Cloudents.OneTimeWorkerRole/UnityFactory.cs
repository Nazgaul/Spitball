using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

//using Zbang.Zbox.WorkerRole.OneTimeUpdates;

namespace Zbang.Cloudents.OneTimeWorkerRole
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

        public Zbang.Zbox.Infrastructure.Ioc.IocFactory Unity { get; private set; }
        public UnityFactory()
        {
            Unity = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;


            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();
            Zbang.Zbox.ReadServices.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

           

            //Unity = new UnityContainer();
            RegisterTypes();
            
        }

        private void RegisterTypes()
        {

            //Unity.RegisterType<IUpdateThumbnails, UpdateThumbnails>();

        }
       
        public T Resolve<T>()
        {
            return Unity.Resolve<T>();
        }
    }
}
