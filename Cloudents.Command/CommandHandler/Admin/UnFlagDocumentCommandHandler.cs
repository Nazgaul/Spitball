using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UnFlagDocumentCommandHandler : ICommandHandler<UnFlagDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public UnFlagDocumentCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }
        public async Task ExecuteAsync(UnFlagDocumentCommand message, CancellationToken token)
        {
            foreach (var id in message.DocumentIds)
            {
                var document = await _documentRepository.LoadAsync(id, token);
                document.UnFlag();
                await _documentRepository.UpdateAsync(document, token);
            }
        }
    }
}
