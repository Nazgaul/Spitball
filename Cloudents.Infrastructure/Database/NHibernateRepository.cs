using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Database
{
    public sealed class NHibernateRepository<T> : IRepository<T> where T : class
    {
        private readonly ISession _session;
        private readonly IUnitOfWork _unitOfWork;

        public NHibernateRepository(UnitOfWork.Factory unitOfWork)
        {
            var att = typeof(T).GetCustomAttribute<DbAttribute>();
            _unitOfWork = unitOfWork.Invoke(att?.Database ?? Core.Enum.Database.System);
            _session = _unitOfWork.Session;
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

        public Task<object> SaveAsync(T entity, CancellationToken token)
        {
            _unitOfWork.FlagCommit();
            return _session.SaveAsync(entity, token);
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _session?.Dispose();
        }
    }
}
