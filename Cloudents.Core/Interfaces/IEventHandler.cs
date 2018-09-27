using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IEventHandler<in T> where T : IEvent
    {
        Task HandleAsync(T eventMessage, CancellationToken token);
    }
}