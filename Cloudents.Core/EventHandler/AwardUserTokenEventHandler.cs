using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    public class AwardUserTokenEventHandler : IEventHandler<DocumentCreatedEvent>
    {
        public Task HandleAsync(DocumentCreatedEvent eventMessage, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}