using Cloudents.Core.DTOs.SearchSync;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Query;
using Cloudents.Query.Sync;
using Cloudents.Search.University;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.Sync
{
    public class UniversityDbToSearchSync : IDbToSearchSync
    {
        private readonly IQueryBus _bus;

        public UniversityDbToSearchSync(IQueryBus bus)
        {
            _bus = bus;
        }


        public async Task<SyncResponse> DoSyncAsync(SyncAzureQuery query, IBinder binder, CancellationToken token)
        {

            var (update, delete, version) =
                await _bus.QueryAsync<(IEnumerable<UniversitySearchDto> update, IEnumerable<string> delete, long version)>(query, token);



            var syncService = await binder.BindAsync<
                IAsyncCollector<AzureSearchSyncOutput>>(
                new AzureSearchSyncAttribute(UniversitySearchWrite.IndexName), token);

            var needContinue = false;
            foreach (var s in update)
            {

                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Search.Entities.University
                    {
                        Name = s.Name,
                        Id = s.UniversityId,
                        Country = s.Country.ToUpperInvariant(),
                        Extra = s.Extra,
                        DisplayName = s.Name,
                        Prefix = new[] { s.Name, s.Extra }.Where(x => x != null).ToArray(),
                        Image = s.Image,
                        UsersCount = s.UsersCount,
                        InsertDate = DateTime.UtcNow
                    },
                    Insert = true

                }, token);
            }

            foreach (var questionDelete in delete)
            {
                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Search.Entities.University
                    {
                        Id = questionDelete
                    },
                    Insert = false

                }, token);
            }

            await syncService.FlushAsync(token);

            return new SyncResponse(version, needContinue);
        }
    }
}