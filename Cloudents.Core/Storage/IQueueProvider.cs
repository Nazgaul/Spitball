using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage.Dto;

namespace Cloudents.Core.Storage
{
    public interface IQueueProvider
    {
        Task InsertQuestionMessageAsync(NewQuestionMessage obj, CancellationToken token);
    }
}