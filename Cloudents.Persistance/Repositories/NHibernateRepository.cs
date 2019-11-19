using Cloudents.Core.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {
        protected readonly ISession Session;


        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "We can initialize this class as well")]
        public NHibernateRepository(ISession session)
        {
            Session = session;
        }

        public virtual Task<object> AddAsync(T entity, CancellationToken token)
        {
            return Session.SaveAsync(entity, token);
        }

        public async Task AddAsync(IEnumerable<T> entities, CancellationToken token)
        {
            foreach (var entity in entities)
            {
                await AddAsync(entity, token);
            }
        }

        public virtual Task<T> LoadAsync(object id, CancellationToken token)
        {
            return Session.LoadAsync<T>(id, token);
        }

        public virtual Task<T> GetAsync(object id, CancellationToken token)
        {
            return Session.GetAsync<T>(id, token);
        }

        public virtual T Load(object id)
        {
            return Session.Load<T>(id);
        }

        public virtual Task DeleteAsync(T entity, CancellationToken token)
        {
            return Session.DeleteAsync(entity, token);
        }

        public Task UpdateAsync(T entity, CancellationToken token)
        {
            return Session.UpdateAsync(entity, token);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Session.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
