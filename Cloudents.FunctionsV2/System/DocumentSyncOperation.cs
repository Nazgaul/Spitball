using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message.System;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Search.Document;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class DocumentSyncOperation : ISystemOperation<DocumentSearchMessage>
    {
        public async Task DoOperationAsync(DocumentSearchMessage msg, IBinder binder, CancellationToken token)
        {
            var syncService = await binder.BindAsync<
                IAsyncCollector<AzureSearchSyncOutput>>(
                new AzureSearchSyncAttribute(DocumentSearchWrite.IndexName), token);

            var output = new AzureSearchSyncOutput
            {
                Insert = msg.ShouldInsert,
                Item = Search.Entities.Document.FromDto(msg.Document)
            };
           
            await syncService.AddAsync(output, token);
        }
    }
}