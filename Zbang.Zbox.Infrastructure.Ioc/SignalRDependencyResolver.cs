using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Ioc
{
    public class SignalRDependencyResolver : DefaultDependencyResolver
    {
        private IUnityContainer _container;

        public SignalRDependencyResolver()
        {
            _container = IocFactory.Unity.unityContainer;
        }

        public override object GetService(Type serviceType)
        {
            try
            {
                return base.GetService(serviceType) ?? _container.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
            //return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return base.GetServices(serviceType) ?? _container.ResolveAll(serviceType);
            }
            catch
            {
                return null;
            }
           
        }
    }
}
