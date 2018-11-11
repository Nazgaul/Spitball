using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class UpdateDocumentNumberViewsOperation : ISystemOperation<UpdateDocumentNumberOfViews>
    {
        private readonly IDocumentRepository _documentRepository;

        public UpdateDocumentNumberViewsOperation(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task DoOperationAsync(UpdateDocumentNumberOfViews msg, IBinder binder, CancellationToken token)
        {
            await _documentRepository.UpdateNumberOfViews(msg.Id, token);
        }
    }
}