using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity, CancellationToken token);
    }
}