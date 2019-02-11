﻿using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
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

            await _repository.DeleteAsync(document, token);

        }
    }
}
