using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentDbRepository<T> where T : class
    {
        Task<T> GetItemAsync(string id);
    }
}
