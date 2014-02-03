using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using Microsoft.Practices.Unity;

namespace Zbang.Zbox.Infrastructure.ServiceModel.RestIoc
{
    public class UnityWebServiceHost : WebServiceHost
    {
        protected IUnityContainer _container;

        public UnityWebServiceHost()
        {
        }

        public UnityWebServiceHost(object singletonInstance, params Uri[] baseAddresses)
            : base(singletonInstance, baseAddresses)
        {
        }

        public UnityWebServiceHost(IUnityContainer container, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _container = container;
        }

        protected override void OnOpening()
        {
            Description.Behaviors.Add(new UnityServiceBehaviour(_container));
            base.OnOpening();
        }
    }
}
