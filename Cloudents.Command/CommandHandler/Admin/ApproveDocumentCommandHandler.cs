using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
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
                document.MakePublic();

                _eventStore.Add(new DocumentCreatedEvent(document));
                await _documentRepository.UpdateAsync(document, token);
            }

        }
    }
}