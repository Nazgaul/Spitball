using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UnDeleteDocumentCommandHandler : ICommandHandler<UnDeleteDocumentCommand>
    {
        private readonly IRepository<Core.Entities.Document> _documentRepository;


        public UnDeleteDocumentCommandHandler(IRepository<Core.Entities.Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(UnDeleteDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.LoadAsync(message.Id, token);
            document.UnDelete();
            await _documentRepository.UpdateAsync(document, token);
        }
    }
}
