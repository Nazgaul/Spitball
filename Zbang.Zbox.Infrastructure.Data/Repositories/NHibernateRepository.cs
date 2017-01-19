using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Repositories;
using NHibernate;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Trace;
using System;
using System.Linq;

namespace Zbang.Zbox.Infrastructure.Data.Repositories
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {

        protected ISession Session => UnitOfWork.CurrentSession;

        protected virtual ISessionFactory SessionFactory => UnitOfWork.CurrentSession.GetSessionImplementation().Factory;

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

        public void Delete(T item, bool flush = false)
        {
            Session.Delete(item);
            if (flush)
            {
                Session.Flush();
            }
        }

        public IQueryable<T> Query()
        {
            return Session.Query<T>();
        }

        //public IQueryable<T> ToList()
        //{
        //    return (from entity in Session.Linq<T>() select entity);
        //}


        public void Save(System.Collections.Generic.IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Session.Save(item);
            }
            
        }


        public void Load(object id, T entity)
        {
            Session.Load(typeof(T), id, LockMode.Upgrade);
        }
    }
}
