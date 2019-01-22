using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Documents.Delete
{
    public class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand>
    {
        private readonly IRepository<Document> _repository;
        private readonly IBlobProvider<DocumentContainer> _blobProvider;
        private readonly IRepository<RegularUser> _userRepository;


        public DeleteDocumentCommandHandler(IRepository<Document> repository,
            IBlobProvider<DocumentContainer> blobProvider, IRepository<RegularUser> userRepository)
        {
            _repository = repository;
            _blobProvider = blobProvider;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(DeleteDocumentCommand message, CancellationToken token)
        {
            var document = await _repository.GetAsync(message.Id, token).ConfigureAwait(false); // no point in load next line will do a query
            if (document == null)
            {
                throw new ArgumentException("document doesn't exists");
            }

            if (document.Status.State != ItemState.Ok)
            {
                throw new ArgumentException("document doesn't exists");

            }
            if (document.Transactions.Count > 0)
            {
                throw new InvalidOperationException("document already purchased");
            }

            if (document.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who uploaded the document");
            }


            if (!(document.User.Actual is RegularUser user))
            {
                throw new InvalidOperationException("cannot delete fictive user");

            }



            await _repository.DeleteAsync(document, token);
            await _blobProvider.DeleteDirectoryAsync(document.Id.ToString());

        }
    }
}
