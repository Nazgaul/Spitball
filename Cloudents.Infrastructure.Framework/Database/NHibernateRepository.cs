using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {
        private readonly ISession _session;
        private readonly IUnitOfWork _unitOfWork;

        public NHibernateRepository(IUnitOfWork unitOfWork)
        {
            _session = unitOfWork.Session;
            _unitOfWork = unitOfWork;
        }

        public Task<T> LoadAsync(object id, CancellationToken token)
        {
            return _session.LoadAsync<T>(id, token);
        }

        public Task<T> GetAsync(object id, CancellationToken token)
        {
            return _session.GetAsync<T>(id, token);
        }

        public IQueryable<T> GetQueryable()
        {
           return _session.Query<T>();
        }

        public Task<object> AddAsync(T entity, CancellationToken token)
        {
            _unitOfWork.FlagCommit();
            return _session.SaveAsync(entity, token);
        }

       
    }
}
