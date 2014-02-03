using System;
using System.ServiceModel;

using Microsoft.Practices.Unity;

namespace Zbang.Zbox.Infrastructure.ServiceModel.Ioc
{
    public class UnityServiceHost: ServiceHost
    {
        //Fields
        private readonly IUnityContainer m_IoCContainer;

        //Ctor
        public UnityServiceHost(IUnityContainer container, Type serviceType, params Uri[] baseAddresses): base(serviceType, baseAddresses)
        {
            m_IoCContainer = container;
        }

        //Methods
        protected override void OnOpening()
        {
            base.OnOpening();

            if (Description.Behaviors.Find<UnityServiceBehavior>() == null)
            {
                Description.Behaviors.Add(new UnityServiceBehavior(m_IoCContainer));
            }
        }
    }
}
