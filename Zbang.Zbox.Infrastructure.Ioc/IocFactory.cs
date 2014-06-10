using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Zbang.Zbox.Infrastructure.Ioc
{
    public class IocFactory
    {
        private static readonly Lazy<IocFactory> Instance
           = new Lazy<IocFactory>(() => new IocFactory());

        private readonly IUnityContainer m_Container = new UnityContainer();
        private static readonly List<Type> RegisteredTypes = new List<Type>();


        internal static ReadOnlyCollection<Type> RegisterTypes
        {
            get { return RegisteredTypes.AsReadOnly(); }
        }
        //private IocFactory()
        //{
        //    //this cause issue with register with name
        //    //m_Container.AddNewExtension<DecoratorContainerExtension>();
            
        //    if (IsOnWeb())
        //    {
        //        HttpContextBase context = new HttpContextWrapper(HttpContext.Current);

        //        context.ApplicationInstance.Disposed += (s, e) =>
        //        {
        //            m_Container.Dispose();
        //        };
        //    }
        //}

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            RegisteredTypes.Add(typeof(TFrom));
            m_Container.RegisterType<TFrom, TTo>();
        }

        public IocFactory RegisterType(Type from, Type to)
        {
            RegisteredTypes.Add(@from);
            m_Container.RegisterType(from, to);

            return this;
        }
        public IocFactory RegisterType(Type from, Type to, string name)
        {
            RegisteredTypes.Add(@from);

            m_Container.RegisterType(from, to, name);
            return this;
        }

        public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            RegisteredTypes.Add(typeof(TFrom));
            m_Container.RegisterType<TFrom, TTo>(name);
        }
        public void RegisterType<TFrom, TTo>(params string[] names) where TTo : TFrom
        {
            foreach (var name in names)
            {
                RegisterType<TFrom, TTo>(name);
            }
        }




        public void RegisterType<TFrom, TTo>(LifeTimeManager lifetypeManager) where TTo : TFrom
        {
            RegisteredTypes.Add(typeof(TFrom));
            m_Container.RegisterType<TFrom, TTo>(GetLifeTimeManager(lifetypeManager));
        }

        //this cause issue with register with name
        //public void RegisterType<TFrom, TDecorator, TBase>(LifeTimeManager lifetypeManager)
        //    where TDecorator : TFrom
        //    where TBase : TFrom
        //{
        //    m_RegisteredTypes.Add(typeof(TFrom));
        //    //var randomString = Guid.NewGuid().ToString();
        //    //m_Container.RegisterType<TFrom, TBase>(randomString);
        //    //m_Container.RegisterType<TFrom, TDecorator>(
        //    //        new InjectionConstructor(new ResolvedParameter(typeof(TFrom), randomString)));

        //    //m_Container.RegisterType<TFrom,TDecorator>(new InjectionFactory(c=> new TDecorator
        //    //m_RegisteredTypes.Add(typeof(TFrom));
            
        //    m_Container.RegisterType<TFrom, TDecorator>(GetLifeTimeManager(lifetypeManager))
        //        .RegisterType<TFrom, TBase>(GetLifeTimeManager(lifetypeManager));
        //}


        public void RegisterType<TFrom, TTo>(string name, LifeTimeManager lifetypeManager) where TTo : TFrom
        {
            RegisteredTypes.Add(typeof(TFrom));
            m_Container.RegisterType<TFrom, TTo>(name, GetLifeTimeManager(lifetypeManager));
        }

        public void RegisterInstance<TFrom>(TFrom instance)
        {
            m_Container.RegisterInstance(instance);
        }


        public T Resolve<T>()
        {
            return m_Container.Resolve<T>();
        }
        public T Resolve<T>(string name)
        {
            return m_Container.Resolve<T>(name);
        }
        public IEnumerable<T> ResolveAll<T>()
        {
            return m_Container.ResolveAll<T>();
        }

        public T Resolve<T>(string name, IocParameterOverride parameters)
        {
            ParameterOverride parms = new ParameterOverride(parameters.Name, parameters.Value);
            return m_Container.Resolve<T>(name, parms);
        }

        public static IocFactory Unity
        {
            get
            {
                return Instance.Value;
            }
        }

        public IUnityContainer UnityContainer
        {
            get { return m_Container; }
        }

        //private bool IsOnWeb()
        //{
        //    return HttpContext.Current != null;
        //}

        private LifetimeManager GetLifeTimeManager(LifeTimeManager manager)
        {
            switch (manager)
            {
                case LifeTimeManager.PerHttpRequest:
                    return new PerHttpRequestLifetime();
                case LifeTimeManager.Singleton:
                    return new ContainerControlledLifetimeManager();
                default:
                    return new PerResolveLifetimeManager();
            }
        }


    }
}
