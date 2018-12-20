using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Application.CommandHandler.Admin
{
    public class ApproveDocumentCommandHandler : ICommandHandler<ApproveDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly IEventStore _eventStore;

        public ApproveDocumentCommandHandler(IRepository<Document> documentRepository, IEventStore eventStore)
        {
            _documentRepository = documentRepository;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(ApproveDocumentCommand message, CancellationToken token)
        {
            foreach (var id in message.Id)
            {
                var document = await _documentRepository.LoadAsync(id, token);
                document.Item.State = ItemState.Ok;

                _eventStore.Add(new DocumentCreatedEvent(document));
                await _documentRepository.UpdateAsync(document, token);
            }

        }
    }
}