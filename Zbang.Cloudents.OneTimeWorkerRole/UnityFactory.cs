
//using Zbang.Zbox.WorkerRole.OneTimeUpdates;

using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Cloudents.OneTimeWorkerRole
{
    internal class UnityFactory
    {
        // UnityContainer unityFactory;
     

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
            Unity.RegisterType<IOneTimeDbi, OneTimeDbi>();

        }
       
        public T Resolve<T>()
        {
            return Unity.Resolve<T>();
        }
    }
}
