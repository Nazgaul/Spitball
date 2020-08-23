using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Documents;

namespace Cloudents.Web.Services
{
    public interface IDocumentGenerator
    {
        Task<object> GeneratePreviewAsync(DocumentDetailDto model, long userId, CancellationToken token);
    }
}