//using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web;
using Autofac;
using Autofac.Builder;


namespace Zbang.Zbox.Infrastructure.Ioc
{
    public class IocFactory : IDisposable
    {
        private static readonly Lazy<IocFactory> Instance
           = new Lazy<IocFactory>(() => new IocFactory());

        //private readonly IUnityContainer m_Container = new UnityContainer();
       public readonly ContainerBuilder BuilderContainer = new ContainerBuilder();
        //private static readonly List<Type> RegisteredTypes = new List<Type>();
        private IContainer m_Container;


        public IContainer Build()
        {
            m_Container = BuilderContainer.Build();
            return m_Container;
        }


        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            //RegisteredTypes.Add(typeof(TFrom));

            RegisterType(typeof(TFrom), typeof(TTo));
            //m_Container.RegisterType(typeof (TTo)).As(typeof (TFrom));
            //m_Container.RegisterType<TFrom, TTo>();
        }

        public void RegisterGeneric(Type from, Type to)
        {
            BuilderContainer.RegisterGeneric(to).As(from);
        }

        public IocFactory RegisterType(Type from, Type to)
        {
            //RegisteredTypes.Add(@from);

            BuilderContainer.RegisterType(to).As(from);
            //m_Container.RegisterType(from, to);

            return this;
        }
        //public IocFactory RegisterType(Type from, Type to, string name)
        //{
        //    //RegisteredTypes.Add(@from);
            
        //    BuilderContainer.RegisterType(to).Named(name, to);//.InstancePerLifetimeScope();
        //    //m_Container.RegisterType(from, to, name);
        //    return this;
        //}

        public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            //RegisteredTypes.Add(typeof(TFrom));
            //m_Container.RegisterType<TFrom, TTo>(name);
            BuilderContainer.RegisterType<TTo>().Named<TFrom>(name);
            //RegisterType(typeof(TFrom), typeof(TTo), name);
        }
        //public void RegisterType<TFrom, TTo>(params string[] names) where TTo : TFrom
        //{
        //    foreach (var name in names)
        //    {
        //        RegisterType<TFrom, TTo>(name);
        //    }
        //}




        public void RegisterType<TFrom, TTo>(LifeTimeManager lifetypeManager) where TTo : TFrom
        {
            //RegisteredTypes.Add(typeof(TFrom));
            switch (lifetypeManager)
            {
                case LifeTimeManager.PerHttpRequest:
                    RegisterType<TFrom, TTo>();
                    //BuilderContainer.RegisterType(typeof(TTo)).As(typeof(TFrom)).InstancePerRequest();
                    break;
                case LifeTimeManager.Singleton:
                    BuilderContainer.RegisterType(typeof(TTo)).As(typeof(TFrom)).InstancePerLifetimeScope();
                    break;
                default:
                    RegisterType<TFrom, TTo>();
                    break;

            }
            //m_Container.RegisterType<TFrom, TTo>(GetLifeTimeManager(lifetypeManager));
        }

        public void RegisterType<TFrom, TTo>(string name, LifeTimeManager lifetypeManager) where TTo : TFrom
        {
            switch (lifetypeManager)
            {
                case LifeTimeManager.PerHttpRequest:
                    RegisterType<TFrom, TTo>(name);
                    //BuilderContainer.RegisterType(typeof(TTo)).Named(name, typeof(TFrom)).InstancePerRequest();
                    break;
                case LifeTimeManager.Singleton:
                    BuilderContainer.RegisterType(typeof(TTo)).Named(name, typeof(TFrom)).InstancePerLifetimeScope();
                    break;
                default:
                    RegisterType<TFrom, TTo>(name);
                    break;

            }
            //RegisteredTypes.Add(typeof(TFrom));
            //m_Container.RegisterType<TFrom, TTo>(name, GetLifeTimeManager(lifetypeManager));
        }

        public void RegisterInstance<TFrom>(TFrom instance) where TFrom : class
        {

            BuilderContainer.RegisterInstance(instance);
        }


        public T Resolve<T>()
        {
            return m_Container.Resolve<T>();
        }
        public T Resolve<T>(string name)
        {
            
            return m_Container.ResolveNamed<T>(name);
            //return m_Container.Resolve<T>(name);
        }
        public IEnumerable<T> ResolveAll<T>()
        {
            return m_Container.Resolve<IEnumerable<T>>();
            //return m_Container.ResolveAll<T>();
        }

        public T Resolve<T>(string name, IocParameterOverride parameters)
        {
            return m_Container.Resolve<T>(new NamedParameter(parameters.Name, parameters.Value));
            //var parms = new ParameterOverride(parameters.Name, parameters.Value);
            //return m_Container.Resolve<T>(name, parms);
        }

        public static IocFactory Unity
        {
            get
            {
                return Instance.Value;
            }
        }

        //public IContainer UnityContainer
        //{
        //    get { return m_Container; }
        //}

        //private bool IsOnWeb()
        //{
        //    return HttpContext.Current != null;
        //}

        //private Func<IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>> GetLifeTimeManager(LifeTimeManager manager)
        //{
        //    switch (manager)
        //    {
        //        case LifeTimeManager.PerHttpRequest:
        //            //return new PerHttpRequestLifetime();
        //        case LifeTimeManager.Singleton:

        //            //return new ContainerControlledLifetimeManager();
        //        default:
        //           // return new PerResolveLifetimeManager();
        //    }
        //}



        //public void Dispose()
        //{

        //    m_Container.Dispose();
        //}

        public void Dispose()
        {
            if (m_Container != null)
            {
                m_Container.Dispose();
            }
        }
    }
}
