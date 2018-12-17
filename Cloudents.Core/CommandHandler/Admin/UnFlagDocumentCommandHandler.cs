﻿using Cloudents.Core.Command.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler.Admin
{
    class UnFlagDocumentCommandHandler : ICommandHandler<UnFlagDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public UnFlagDocumentCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }
        public async Task ExecuteAsync(UnFlagDocumentCommand message, CancellationToken token)
        {
            foreach (var documentId in message.DocumentIds)
            {
                var document = await _documentRepository.LoadAsync(documentId, token);
                document.Item.State = ItemState.Ok;
                document.Item.FlagReason = null;
                document.Item.Votes.Clear();

                await _documentRepository.UpdateAsync(document, token);
            }

        }
    }
}
