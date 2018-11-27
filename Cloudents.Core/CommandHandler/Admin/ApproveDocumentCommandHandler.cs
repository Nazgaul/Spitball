using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class ApproveDocumentCommandHandler : ICommandHandler<ApproveDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public ApproveDocumentCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(ApproveDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.LoadAsync(message.Id, token);
            document.State = ItemState.Ok;
            await _documentRepository.UpdateAsync(document, token);
        }
    }
}