using Cloudents.Core.Message.System;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Search;
using Cloudents.Search.Entities;
using Cloudents.Search.Question;

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
                Item = new Question(msg.Question),
                Insert = msg.ShouldInsert
            };
            await syncService.AddAsync(output, token);
        }
    }
}