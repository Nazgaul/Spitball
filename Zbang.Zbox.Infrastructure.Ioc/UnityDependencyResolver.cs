using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Zbox.Infrastructure.Ioc
{
    public class UnityDependencyResolver : IDependencyResolver,System.Web.Http.Dependencies.IDependencyResolver 
    {
        private IUnityContainer _container;

        public UnityDependencyResolver()
        {
            _container = IocFactory.Unity.unityContainer;
            //RegisterDisposeOfChildContainer();
        }

        //public object GetService(Type serviceType)
        //{
        //    try
        //    {
        //        if (serviceType.Namespace == "System.Web.Mvc")
        //        {
        //            return null;
        //        }

        //        //if (serviceType == typeof(IController) || serviceType == typeof(IControllerFactory) || serviceType == typeof(IControllerActivator))
        //        //{
        //        //    return null;
        //        //}
        //        return _container.Resolve(serviceType);


        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public IEnumerable<object> GetServices(Type serviceType)
        //{
        //    try
        //    {
        //        return _container.ResolveAll(serviceType);
        //    }
        //    catch (Exception)
        //    {
        //        return new List<object>();
        //    }
        //}


        private const string HttpContextKey = "perRequestContainer";

        //private readonly IUnityContainer _container;

        //public UnityDependencyResolver(IUnityContainer container)
        //{
        //    _container = container;
        //}

        public object GetService(Type serviceType)
        {
            if (typeof(IController).IsAssignableFrom(serviceType) 
                || typeof(System.Web.Http.Controllers.IHttpController).IsAssignableFrom(serviceType))
            {
                return ChildContainer.Resolve(serviceType);
            }

            return IsRegistered(serviceType) ?
                ChildContainer.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (IsRegistered(serviceType))
            {
                yield return ChildContainer.Resolve(serviceType);
            }

            foreach (var service in ChildContainer.ResolveAll(serviceType))
            {
                yield return service;
            }
        }

        protected IUnityContainer ChildContainer
        {
            get
            {
                return _container;
                //var childContainer = HttpContext.Current.Items[HttpContextKey] as IUnityContainer;

                //if (childContainer == null)
                //{
                //    HttpContext.Current.Items[HttpContextKey] = childContainer = _container.CreateChildContainer();
                //    HttpContext.Current.
                //}

                //return childContainer;
            }
        }

        //private void RegisterDisposeOfChildContainer()
        //{
        //    HttpContextBase context = new HttpContextWrapper(HttpContext.Current);

        //    context.ApplicationInstance.EndRequest += (s, e) =>
        //    {
        //        var childContainer = HttpContext.Current.Items[HttpContextKey] as IUnityContainer;

        //        if (childContainer != null)
        //        {
        //            childContainer.Dispose();
        //        }
        //    };
        //}

        private bool IsRegistered(Type typeToCheck)
        {
            return IocFactory.RegisterTypes.Any(a => a.FullName == typeToCheck.FullName);
            //var isRegistered = true;

            //if (typeToCheck.IsInterface || typeToCheck.IsAbstract)
            //{

            //    isRegistered = ChildContainer.IsRegistered(typeToCheck);

            //    if (!isRegistered && typeToCheck.IsGenericType)
            //    {
            //        var openGenericType = typeToCheck.GetGenericTypeDefinition();

            //        isRegistered = ChildContainer.IsRegistered(openGenericType);
            //    }
            //}

            //return isRegistered;
        }



        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
           
        }
    }
}
