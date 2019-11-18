using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveDocumentCommandHandler : ICommandHandler<ApproveDocumentCommand>
    {
        private readonly IRepository<Core.Entities.Document> _documentRepository;

        public ApproveDocumentCommandHandler(IRepository<Core.Entities.Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(ApproveDocumentCommand message, CancellationToken token)
        {
            foreach (var id in message.Id)
            {
                var document = await _documentRepository.LoadAsync(id, token);
                document.MakePublic();
                await _documentRepository.UpdateAsync(document, token);
            }

        }
    }
}