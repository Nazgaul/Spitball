﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class UpdateDocumentNumberViewsOperation : ISystemOperation<UpdateDocumentNumberOfViews>
    {
        private readonly ICommandBus _documentRepository;

        public UpdateDocumentNumberViewsOperation(ICommandBus documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task DoOperationAsync(UpdateDocumentNumberOfViews msg, IBinder binder, CancellationToken token)
        {
            var command = new IncrementDocumentNumberOfViewsCommand(msg.Id);
            await _documentRepository.DispatchAsync(command, token);
        }
    }
}