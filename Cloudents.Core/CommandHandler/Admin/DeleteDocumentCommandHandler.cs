using Cloudents.Core.Command.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly IEventStore _eventStore;


        public DeleteDocumentCommandHandler(IRepository<Document> documentRepository, IEventStore eventStore)
        {
            _documentRepository = documentRepository;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(DeleteDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.GetAsync(message.Id, token);
            if (document.Item.State == ItemState.Deleted)
            {
                return;
            }

            _eventStore.Add(new DocumentDeletedEvent(document));
            //document.Events.Add(new DocumentDeletedEvent(document));
            await _documentRepository.DeleteAsync(document, token);


        }
    }
}