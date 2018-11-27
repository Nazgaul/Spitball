using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

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
            var document = await _documentRepository.LoadAsync(message.Id, token);
            document.Events.Add(new DocumentDeletedEvent(document));
            await _documentRepository.DeleteAsync(document, token);


        }
    }
}