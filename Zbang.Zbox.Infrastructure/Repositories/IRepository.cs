using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface IRepository<TEntity>
    {
        TEntity Get(object id);
        TEntity Load(object id);

        void Load(object id, TEntity entity);
        void Save(TEntity item, bool flush = false);
        void Save(IEnumerable<TEntity> items);

        void Delete(TEntity item, bool flush = false);

        IQueryable<TEntity> GetQueryable();
        
    }

    public interface IDocumentDbRepository<T>
    {
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task CreateItemAsync(T item);
        Task UpdateItemAsync(string id, T item);
        Task DeleteItemAsync(string id);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentDbModelAttribute : Attribute
    {
        public DocumentDbModelAttribute(string collectionId)
        {
            CollectionId = collectionId;
        }
        public string CollectionId { get; private set; }
    }
}
