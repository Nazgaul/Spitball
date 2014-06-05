
//using Zbang.Zbox.WorkerRole.OneTimeUpdates;

using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Ioc;

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

        public IocFactory Unity { get; private set; }
        public UnityFactory()
        {
            Unity = IocFactory.Unity;


            RegisterIoc.Register();
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();
            Zbox.ReadServices.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

           

            //Unity = new UnityContainer();
            RegisterTypes();
            
        }

        private void RegisterTypes()
        {

            Unity.RegisterType<IUpdateThumbnails, UpdateThumbnails>();

        }
       
        public T Resolve<T>()
        {
            return Unity.Resolve<T>();
        }
    }
}
