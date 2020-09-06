using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task UpdateNumberOfViewsAsync(long id, CancellationToken token);
        Task UpdateNumberOfDownloadsAsync(long id, CancellationToken token);
    }
}