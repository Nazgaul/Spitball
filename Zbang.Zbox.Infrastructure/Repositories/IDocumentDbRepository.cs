using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface IDocumentDbRepository<T>
    {
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task CreateItemAsync(T item);
        Task UpdateItemAsync(string id, T item);
        Task DeleteItemAsync(string id);

        Task<IEnumerable<T>> GetItemsAsync(string sql);
    }
}