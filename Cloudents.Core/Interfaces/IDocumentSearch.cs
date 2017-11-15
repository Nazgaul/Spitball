using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentSearch
    {
        Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken);
    }
}