using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web.Mvc;

namespace Zbang.Zbox.Infrastructure.Web.Ioc
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        #region Members

        private IUnityContainer _container;

        #endregion

        #region Ctor

        public UnityDependencyResolver(IUnityContainer container)
        {
            _container = container;

        }

        #endregion

        #region IDependencyResolver Members

        public object GetService(Type serviceType)
        {
            try
            {
                if (serviceType.Namespace == "System.Web.Mvc")
                {
                    return null;
                }

                //if (serviceType == typeof(IController) || serviceType == typeof(IControllerFactory) || serviceType == typeof(IControllerActivator))
                //{
                //    return null;
                //}
                return _container.Resolve(serviceType);


            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }

        #endregion



    }
}
