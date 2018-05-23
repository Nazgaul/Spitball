using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {
        protected readonly ISession Session;
        private readonly IUnitOfWork _unitOfWork;

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Ioc inject")]
        public NHibernateRepository(UnitOfWork.Factory unitOfWork)
        {
            var att = typeof(T).GetCustomAttribute<DbAttribute>();
            _unitOfWork = unitOfWork.Invoke(att?.Database ?? Core.Enum.Database.System);
            Session = _unitOfWork.Session;
        }

        public Task<T> LoadAsync(object id, CancellationToken token)
        {
            return Session.LoadAsync<T>(id, token);
        }

        public Task<T> GetAsync(object id, CancellationToken token)
        {
            return Session.GetAsync<T>(id, token);
        }

        public IQueryable<T> GetQueryable()
        {
           return Session.Query<T>();
        }

        public Task<object> SaveAsync(T entity, CancellationToken token)
        {
            _unitOfWork.FlagCommit();
            return Session.SaveAsync(entity, token);
        }
    }
}
