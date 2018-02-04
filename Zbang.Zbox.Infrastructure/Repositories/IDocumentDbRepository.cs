using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface IDocumentDbRepository<T>
    {
        Task<T> GetItemAsync(string id);
        Task CreateItemAsync(T item);
        Task UpdateItemAsync(string id, T item);

    }
}