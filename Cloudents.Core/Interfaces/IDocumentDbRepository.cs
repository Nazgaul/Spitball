using System.Threading.Tasks;

namespace Cloudents.Application.Interfaces
{
    public interface IDocumentDbRepository<T> where T : class
    {
        Task<T> GetItemAsync(string id);
    }
}
