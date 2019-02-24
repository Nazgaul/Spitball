using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateDocumentMetaCommandHandler : ICommandHandler<UpdateDocumentMetaCommand>
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly ITagRepository _tagRepository;

        public UpdateDocumentMetaCommandHandler(IRepository<Document> documentRepository, ITagRepository tagRepository)
        {
            _documentRepository = documentRepository;
            _tagRepository = tagRepository;
        }

        public async Task ExecuteAsync(UpdateDocumentMetaCommand message, CancellationToken token)
        {

            var doc = await _documentRepository.LoadAsync(message.Id, token);
            doc.PageCount = message.PageCount;
            doc.MetaContent = message.Snippet;
            foreach (var tagStr in message.Tags)
            {
                var tag = await _tagRepository.GetOrAddAsync(tagStr, token);
                doc.Tags.Add(tag);
            }
            await _documentRepository.UpdateAsync(doc, token);
        }
    }
}