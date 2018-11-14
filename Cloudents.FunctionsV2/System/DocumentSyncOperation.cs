using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class DocumentSyncOperation : ISystemOperation<DocumentSearchMessage>
    {
        private readonly ISearchServiceWrite<Document> _documentServiceWrite;

        public DocumentSyncOperation(ISearchServiceWrite<Document> documentServiceWrite)
        {
            _documentServiceWrite = documentServiceWrite;
        }

        public Task DoOperationAsync(DocumentSearchMessage msg, IBinder binder, CancellationToken token)
        {
            return _documentServiceWrite.UpdateDataAsync(new[] { msg.Document }, token);
        }
    }
}