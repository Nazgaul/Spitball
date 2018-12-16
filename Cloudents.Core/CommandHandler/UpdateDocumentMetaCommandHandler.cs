using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class UpdateDocumentMetaCommandHandler : ICommandHandler<UpdateDocumentMetaCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public UpdateDocumentMetaCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(UpdateDocumentMetaCommand message, CancellationToken token)
        {
            //TODO: change this to update and check if the upadte occure
            var doc = await _documentRepository.LoadAsync(message.Id, token);
            doc.PageCount = message.PageCount;
            doc.MetaContent = message.Snippet;

            await _documentRepository.UpdateAsync(doc, token);
        }
    }
}