using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;

namespace Cloudents.Functions.Sync
{
    public class UniversityDbToSearchSync : IDbToSearchSync
    {
        private readonly ISearchServiceWrite<University> _universityServiceWrite;
        private readonly IQueryBus _bus;

        public UniversityDbToSearchSync(ISearchServiceWrite<University> universityServiceWrite, IQueryBus bus)
        {
            _universityServiceWrite = universityServiceWrite;
            _bus = bus;
        }

        public Task CreateIndexAsync(CancellationToken token)
        {
            return _universityServiceWrite.CreateOrUpdateAsync(token);
        }

        public async Task<SyncResponse> DoSyncAsync(SyncAzureQuery query, CancellationToken token)
        {
            var stopWordsList = new[]{ "university",
                "of",
                "college",
                "school",
                "the",
                "a",
                "המכללה","אוניברסיטת","מכללת","האוניברסיטה"
            };
            var (update, delete, version) =
                await _bus.QueryAsync<(IEnumerable<UniversitySearchDto> update, IEnumerable<long> delete, long version)>(query, token);
            var result = await _universityServiceWrite.UpdateDataAsync(update.Select(s =>
            {
                
                var cleanName = s.Name.RemoveWords(stopWordsList);

                return new University
                {
                    Name = cleanName,
                    Id = s.Id,
                    Country = s.Country,
                    Extra = s.Extra,
                    DisplayName = s.Name,
                    Prefix = new[] { cleanName, s.Extra}.Where(x => x != null).ToArray(),
                };
            }), delete.Select(s => s.ToString()), token);
            return new SyncResponse(version, result);
        }
    }
}