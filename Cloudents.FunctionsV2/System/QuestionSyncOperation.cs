using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class QuestionSyncOperation : ISystemOperation<QuestionSearchMessage>
    {

        public async Task DoOperationAsync(QuestionSearchMessage msg, IBinder binder, CancellationToken token)
        {
            var syncService = await binder.BindAsync<
                IAsyncCollector<AzureSearchSyncOutput>>(
                    new AzureSearchSyncAttribute(QuestionSearchWrite.IndexName), token);

            var output = new AzureSearchSyncOutput
            {
                Item = msg.Question,
                Insert = msg.ShouldInsert
            };
            await syncService.AddAsync(output, token);
        }
    }
}