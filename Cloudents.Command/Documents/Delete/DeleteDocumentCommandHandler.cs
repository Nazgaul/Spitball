using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Documents.Delete
{
    public class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand>
    {
        private readonly IRepository<Document> _repository;


        public DeleteDocumentCommandHandler(IRepository<Document> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(DeleteDocumentCommand message, CancellationToken token)
        {
            var document = await _repository.LoadAsync(message.Id, token); // no point in load next line will do a query

            if (document.Status.State != ItemState.Ok)
            {
                throw new NotFoundException("document doesn't exists");
            }

            if (document.User.Id != message.UserId)
            {
                throw new ArgumentException("user is not the one who uploaded the document");
            }

            if (document.Transactions.Any())
            {
                throw new InvalidOperationException("document already purchased");
            }

            await _repository.DeleteAsync(document, token);
        }
    }
}
