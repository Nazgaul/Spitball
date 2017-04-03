using System;
using Autofac;
using NHibernate;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork
{
    public class UnitOfWork : Autofac.IStartable
    {
        private IUnitOfWorkFactory UnitOfWorkFactory;
        public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";

        //UnitOfWork()
        //{
        //    UnitOfWorkFactory = new UnitOfWorkFactory();
        //}

        private static IUnitOfWork CurrentUnitOfWork
        {
            get { return Local.Data[CurrentUnitOfWorkKey] as IUnitOfWork; }
            set { Local.Data[CurrentUnitOfWorkKey] = value; }
        }

        public static IUnitOfWork Current
        {
            get
            {
                var unitOfWork = CurrentUnitOfWork;
                if (unitOfWork == null)
                {
                    throw new InvalidOperationException("You are not in a unit of work");
                }
                return unitOfWork;
            }
        }

        public static bool IsStarted => CurrentUnitOfWork != null;

        public ISession CurrentSession
        {
            get { return UnitOfWorkFactory.CurrentSession; }
            internal set { UnitOfWorkFactory.CurrentSession = value; }
        }
        void IStartable.Start()
        {
            UnitOfWorkFactory = new UnitOfWorkFactory();
        }
        public IUnitOfWork Start()
        {
            if (CurrentUnitOfWork != null)
            {
                throw new InvalidOperationException("You cannot start more than one unit of work at the same time.");
            }
            var unitOfWork = UnitOfWorkFactory.Create();
            CurrentUnitOfWork = unitOfWork;
            return unitOfWork;
        }

        public static void DisposeUnitOfWork(IUnitOfWorkImplementor unitOfWork)
        {
            CurrentUnitOfWork = null;
            Local.Data.Clear();
        }
    }
}
