using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class RenameDocumentCommandHandler : ICommandHandler<RenameDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public RenameDocumentCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(RenameDocumentCommand message, CancellationToken token)
        {

            var document = await _documentRepository.LoadAsync(message.DocumentId, token);
            if (document.User.Id != message.UserId)
            { 
                throw new ArgumentException("user is not the one who uploaded the document");
            }

            document.Rename(message.Name);

            await _documentRepository.UpdateAsync(document, token);
        }

    }
}
