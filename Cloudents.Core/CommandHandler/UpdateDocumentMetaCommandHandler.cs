using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.CommandHandler
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