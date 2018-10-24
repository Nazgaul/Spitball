using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Sync;

namespace Cloudents.Functions.Sync
{
    public class CourseDbToSearchSync : IDbToSearchSync
    {
        private readonly ISearchServiceWrite<Course> _courseServiceWrite;
        private readonly IQueryBus _bus;

        public CourseDbToSearchSync(ISearchServiceWrite<Course> courseServiceWrite, IQueryBus bus)
        {
            _courseServiceWrite = courseServiceWrite;
            _bus = bus;
        }

        public Task CreateIndexAsync(CancellationToken token)
        {
            return _courseServiceWrite.CreateOrUpdateAsync(token);
        }

        public async Task<SyncResponse> DoSyncAsync(SyncAzureQuery query, CancellationToken token)
        {
            var (update, delete, version) =
                await _bus.QueryAsync<(IEnumerable<CourseSearchDto> update, IEnumerable<string> delete, long version)>(query, token);
            var result = await _courseServiceWrite.UpdateDataAsync(update.Select(s =>
            {

                return new Course
                {
                    Name = s.Name,
                    Id = s.Name.ToString(),
                    Extra = s.Extra.Split(new []{' '},StringSplitOptions.RemoveEmptyEntries),
                    Prefix = new[] { s.Name, s.Extra }.Where(x => x != null).ToArray(),
                };
            }), delete, token);
            return new SyncResponse(version, result);
        }
    }
}