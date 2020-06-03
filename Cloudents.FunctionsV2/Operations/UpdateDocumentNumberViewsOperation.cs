using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.Operations
{
    public class UpdateDocumentNumberViewsOperation : ISystemOperation<UpdateDocumentNumberOfViews>
    {
        private readonly ICommandBus _documentRepository;

        public UpdateDocumentNumberViewsOperation(ICommandBus documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public Task DoOperationAsync(UpdateDocumentNumberOfViews msg, IBinder binder, CancellationToken token)
        {
            var command = new IncrementDocumentNumberOfViewsCommand(msg.Id);
            return _documentRepository.DispatchAsync(command, token);
        }
    }
}