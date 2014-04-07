using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using NHibernate;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Trace;
using System;
using System.Linq;

namespace Zbang.Zbox.Infrastructure.Data.Repositories
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {

        protected ISession Session
        {
            get
            { return UnitOfWork.CurrentSession; }
        }
        protected virtual ISessionFactory SessionFactory
        {
            get { return UnitOfWork.CurrentSession.GetSessionImplementation().Factory; }
        }

        protected ICriteria CreateCriteria()
        {
            return Session.CreateCriteria<T>();
        }

        protected IQueryOver<T, T> CreateQueryOver()
        {
            return Session.QueryOver<T>();
        }

        public T Get(object id)
        {
            return Session.Get<T>(id);
        }
        public T UnProxyObjectAs(T obj)
        {
            return Session.GetSessionImplementation().PersistenceContext.Unproxy(obj) as T;
        }

        public T Load(object id)
        {
            return Session.Load<T>(id);
        }
        public void Save(T item, bool flush = false)
        {
            try
            {
                Session.Save(item);
                if (flush)
                {
                    Session.Flush();
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Nhiberate save flush error", ex);
                throw;
            }
        }

        public void Save(T item, object id)
        {
            Session.Save(item, id);
        }

        public void Delete(T item)
        {
            Session.Delete(item);
        }

        public IQueryable<T> GetQuerable()
        {
            return Session.Query<T>();
        }
    }
}
