using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventMessage, CancellationToken token) where T : IEvent;
    }
}