using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        Task<object> SaveAsync(T entity, CancellationToken token);
        Task<T> LoadAsync(object id, CancellationToken token);
        Task<T> GetAsync(object id, CancellationToken token);

        IQueryable<T> GetQueryable();
    }
}