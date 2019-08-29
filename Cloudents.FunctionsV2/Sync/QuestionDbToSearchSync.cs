using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Query;
using Cloudents.Query.Query.Sync;
using Cloudents.Search.Question;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.Sync
{
    public class QuestionDbToSearchSync : IDbToSearchSync
    {
        private readonly IQueryBus _bus;

        public QuestionDbToSearchSync( IQueryBus bus)
        {
            _bus = bus;
        }



        //public Task CreateIndexAsync(CancellationToken token)
        //{
        //    return _questionServiceWrite.CreateOrUpdateAsync(token);
        //}

        public async Task<SyncResponse> DoSyncAsync(SyncAzureQuery query,IBinder binder, CancellationToken token)
        {
            var (update, delete, version) =
                await _bus.QueryAsync<(IEnumerable<QuestionSearchDto> update, IEnumerable<string> delete, long version)>(query, token);

            var syncService = await binder.BindAsync<
                IAsyncCollector<AzureSearchSyncOutput>>(
                new AzureSearchSyncAttribute(QuestionSearchWrite.IndexName), token);

            var needContinue = false;
            foreach (var questionUpdate in update)
            {

                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Cloudents.Search.Entities.Question(questionUpdate),
                    Insert = true

                }, token);
            }

            foreach (var questionDelete in delete)
            {
                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Cloudents.Search.Entities.Question(questionDelete),
                    Insert = false

                }, token);
            }

            await syncService.FlushAsync(token);
            //var result = await _questionServiceWrite.UpdateDataAsync(update.Select(s => s.ToQuestion()), delete, token);
            return new SyncResponse(version, needContinue);
        }

       
    }
}