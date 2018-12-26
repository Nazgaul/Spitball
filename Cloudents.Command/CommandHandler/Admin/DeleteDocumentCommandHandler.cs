using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
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
            await _documentRepository.DeleteAsync(document, token);


        }
    }
}