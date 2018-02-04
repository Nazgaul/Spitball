using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageAsync<T>(QueueName name,T message, CancellationToken token);
    }
}