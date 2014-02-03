using System;

using NHibernate.Cfg;
using NHibernate;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork
{
    public static class UnitOfWork
    {
        private static readonly IUnitOfWorkFactory UnitOfWorkFactory;
        public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";

        static UnitOfWork()
        {
            UnitOfWorkFactory = new UnitOfWorkFactory();
        }

        //public static Configuration Configuration
        //{
        //    get { return UnitOfWorkFactory.Configuration; }
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

        public static bool IsStarted
        {
            get { return CurrentUnitOfWork != null; }
        }

        public static ISession CurrentSession
        {
            get { return UnitOfWorkFactory.CurrentSession; }
            internal set { UnitOfWorkFactory.CurrentSession = value; }
        }

        public static IUnitOfWork Start()
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
        }
    }
}
