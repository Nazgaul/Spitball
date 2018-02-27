using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    // ReSharper disable once PossibleInfiniteInheritance T is where defined
    public abstract class SyncAzureSearchRepository<T> : IReadRepositoryAsync<(IEnumerable<T> update, IEnumerable<SearchWriteBaseDto> delete, long version), SyncAzureQuery>
        where T : SearchWriteIsDeleted
    {
        private readonly DapperRepository _repository;

        protected SyncAzureSearchRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        protected abstract string WriteSql { get; }
        protected abstract string DeleteSql { get; }

        public Task<(IEnumerable<T> update, IEnumerable<SearchWriteBaseDto> delete, long version)> GetAsync(SyncAzureQuery query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(async c =>
            {
                using (var grid = await c.QueryMultipleAsync(WriteSql + ";" + DeleteSql, new { version = query.Version, PageNumber = query.Page, PageSize = 50 }).ConfigureAwait(false))
                {
                    var write = (await grid.ReadAsync<T>().ConfigureAwait(false)).ToLookup(p => p.IsDeleted);
                    var delete = await grid.ReadAsync<SearchWriteBaseDto>().ConfigureAwait(false);

                    var deleteList = delete.Union(write[true]
                        .Select(s => new SearchWriteBaseDto { Id = s.Id, Version = s.Version })).ToList();

                    var update = write[false].ToList();
                    long max = 0, maxDelete = 0;
                    if (write.Count > 0)
                    {
                        max = update.Max(m => m.Version);
                    }
                    if (deleteList.Count > 0)
                    {
                        maxDelete = deleteList.Max(m => m.Version);
                    }


                    return (update.AsEnumerable(), deleteList.AsEnumerable(), new[] { max, maxDelete }.Max());
                    //return (update.AsEnumerable(), deleteList.AsEnumerable(), max);
                }
            }, token);
        }
    }
}