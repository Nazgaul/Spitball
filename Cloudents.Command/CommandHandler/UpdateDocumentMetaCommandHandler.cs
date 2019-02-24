using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
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

            var doc = await _documentRepository.LoadAsync(message.Id, token);
            doc.PageCount = message.PageCount;
            doc.MetaContent = message.Snippet;
            foreach (var tag in message.Tags)
            {
                doc.Tags.Add(new Tag(tag));
            }
            await _documentRepository.UpdateAsync(doc, token);
        }
    }
}