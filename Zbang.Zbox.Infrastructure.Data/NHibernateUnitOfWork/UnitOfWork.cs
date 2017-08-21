using System;
using NHibernate;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork
{
    //public interface IUnitOfWork
    //{
        
    //}
    public class UnitOfWork 
    {
        private static readonly IUnitOfWorkFactory UnitOfWorkFactory;
        public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";

        static UnitOfWork()
        {
            UnitOfWorkFactory = new UnitOfWorkFactory();
        }

        private static IUnitOfWork CurrentUnitOfWork
        {
            get => Local.Data[CurrentUnitOfWorkKey] as IUnitOfWork;
            set => Local.Data[CurrentUnitOfWorkKey] = value;
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


        public static ISession CurrentSession
        {
            get => UnitOfWorkFactory.CurrentSession;
            internal set => UnitOfWorkFactory.CurrentSession = value;
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

        public static void DisposeUnitOfWork(IUnitOfWorkImplementer unitOfWork)
        {
            CurrentUnitOfWork = null;
        }
    }
}
