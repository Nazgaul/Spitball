using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public DeleteDocumentCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(DeleteDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.GetAsync(message.Id, token);
            if (document.Item.State == ItemState.Deleted)
            {
                return;
            }

            document.Events.Add(new DocumentDeletedEvent(document));
            await _documentRepository.DeleteAsync(document, token);


        }
    }
}