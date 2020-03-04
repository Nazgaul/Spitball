using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.Operations
{
    public class UpdateDocumentNumberDownloadsOperation : ISystemOperation<UpdateDocumentNumberOfDownloads>
    {
        private readonly ICommandBus _documentRepository;

        public UpdateDocumentNumberDownloadsOperation(ICommandBus documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task DoOperationAsync(UpdateDocumentNumberOfDownloads msg, IBinder binder, CancellationToken token)
        {
            var command = new IncrementDocumentNumberOfDownloadsCommand(msg.Id);
            await _documentRepository.DispatchAsync(command, token);
        }
    }
}