using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Data.Repositories
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {
        protected readonly ISession Session;
        //private readonly IUnitOfWork _unitOfWork;

        //[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "We can initialize this class as well")]
        //public NHibernateRepository(IIndex<Core.Enum.Database, IUnitOfWork> unitOfWorks)
        //{
        //    var att = typeof(T).GetCustomAttribute<DbAttribute>();
        //    _unitOfWork = unitOfWorks[att?.Database ?? Core.Enum.Database.System];
        //    Session = _unitOfWork.Session;
        //}

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "We can initialize this class as well")]
        public NHibernateRepository(ISession session)
        {
            Session = session;
        }

        //public Task<T> LoadAsync(object id, CancellationToken token)
        //{
        //    return Session.LoadAsync<T>(id, token);
        //}

        public virtual Task<object> AddAsync(T entity, CancellationToken token)
        {
            // _unitOfWork.FlagCommit();
            return Session.SaveAsync(entity, token);
        }

        public async Task AddAsync(IEnumerable<T> entities, CancellationToken token)
        {
            foreach (var entity in entities)
            {
                await AddAsync(entity, token);
            }
        }

        public Task<T> LoadAsync(object id, CancellationToken token)
        {
            return Session.LoadAsync<T>(id, token);
        }

        public Task<T> GetAsync(object id, CancellationToken token)
        {
            return Session.GetAsync<T>(id, token);
        }

        public T Load(object id)
        {
            return Session.Load<T>(id);
        }

        public Task DeleteAsync(T entity, CancellationToken token)
        {
            //_unitOfWork.FlagCommit();
            return Session.DeleteAsync(entity, token);
        }

        public Task UpdateAsync(T entity, CancellationToken token)
        {
            return Session.UpdateAsync(entity, token);
        }
    }
}
