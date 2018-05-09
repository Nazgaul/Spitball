using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageAsync<T>(T message, CancellationToken token) where T : IQueueName;
    }
}