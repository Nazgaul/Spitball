//using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using Autofac;


namespace Zbang.Zbox.Infrastructure.Ioc
{
    public class IocFactory : IDisposable
    {
        private static readonly Lazy<IocFactory> Instance
           = new Lazy<IocFactory>(() => new IocFactory());

        private ContainerBuilder m_BuilderContainer;// = new ContainerBuilder();
        private IContainer m_Container;

        public ContainerBuilder ContainerBuilder
        {
            get { return m_BuilderContainer ?? (m_BuilderContainer = new ContainerBuilder()); }
            set { m_BuilderContainer = value; }
        }



        public IContainer Build()
        {
            m_Container = ContainerBuilder.Build();
            return m_Container;
        }




        //public void RegisterType<TFrom, TTo>() where TTo : TFrom
        //{
        //    RegisterType(typeof(TFrom), typeof(TTo));
        //}

        //public void RegisterGeneric(Type from, Type to)
        //{
        //    ContainerBuilder.RegisterGeneric(to).As(from);
        //}

        //public IocFactory RegisterType(Type from, Type to)
        //{
        //    ContainerBuilder.RegisterType(to).As(from);
        //    return this;
        //}

        //public void RegisterType<T>()
        //{
        //    ContainerBuilder.RegisterType<T>().AsSelf().InstancePerLifetimeScope();
        //}

        //public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        //{
            
        //    ContainerBuilder.RegisterType<TTo>().Named<TFrom>(name);
        //}

        //public void RegisterType<TFrom, TTo>(LifeTimeManager lifetypeManager) where TTo : TFrom
        //{
        //    switch (lifetypeManager)
        //    {
        //        case LifeTimeManager.PerHttpRequest:
        //            RegisterType<TFrom, TTo>();
        //            //BuilderContainer.RegisterType(typeof(TTo)).As(typeof(TFrom)).InstancePerRequest();
        //            break;
        //        case LifeTimeManager.Singleton:
        //            ContainerBuilder.RegisterType(typeof(TTo)).As(typeof(TFrom)).SingleInstance();
        //            break;
        //        default:
        //            RegisterType<TFrom, TTo>();
        //            break;

        //    }
        //}

        //public void RegisterType<TFrom, TTo>(string name, LifeTimeManager lifetypeManager) where TTo : TFrom
        //{
        //    switch (lifetypeManager)
        //    {
        //        case LifeTimeManager.PerHttpRequest:
        //            RegisterType<TFrom, TTo>(name);
        //            //BuilderContainer.RegisterType(typeof(TTo)).Named(name, typeof(TFrom)).InstancePerRequest();
        //            break;
        //        case LifeTimeManager.Singleton:
        //            ContainerBuilder.RegisterType(typeof(TTo)).Named(name, typeof(TFrom)).InstancePerLifetimeScope();
        //            break;
        //        default:
        //            RegisterType<TFrom, TTo>(name);
        //            break;

        //    }
        //}

        public void RegisterInstance<TFrom>(TFrom instance) where TFrom : class
        {

            ContainerBuilder.RegisterInstance(instance);
        }


        public T Resolve<T>()
        {
            return m_Container.Resolve<T>();
        }
        //public T Resolve<T>(string name)
        //{

        //    return m_Container.ResolveNamed<T>(name);
        //}

        //public T TryResolve<T>(string name) where T: class
        //{
        //    return m_Container.ResolveOptionalNamed<T>(name);
        //}

        //public IEnumerable<T> ResolveAll<T>()
        //{
        //    return m_Container.Resolve<IEnumerable<T>>();
        //}

        //public T Resolve<T>(string name, IocParameterOverride parameters)
        //{
            
        //    return m_Container.ResolveNamed<T>(name, new NamedParameter(parameters.Name, parameters.Value));
        //}


        public static IocFactory IocWrapper => Instance.Value;

        public void Dispose()
        {
            m_Container?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
