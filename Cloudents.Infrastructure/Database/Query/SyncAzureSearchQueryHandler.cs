using Cloudents.Core.DTOs;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Data;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    public abstract class SyncAzureSearchQueryHandler<T> //: 
        //IQueryHandler<SyncAzureQuery<T>, (IEnumerable<T> update, IEnumerable<long> delete, long version)>
        where T : AzureSyncBaseDto
    {
        protected abstract string VersionSql { get; }

        protected abstract string FirstQuery { get; }

        protected readonly QueryBuilder QueryBuilder;
        private readonly DapperRepository _dapperRepository;

        protected SyncAzureSearchQueryHandler(QueryBuilder queryBuilder, DapperRepository dapperRepository)
        {
            QueryBuilder = queryBuilder;
            _dapperRepository = dapperRepository;
        }

        public Task<(IEnumerable<T> update, IEnumerable<long> delete, long version)> GetAsync(SyncAzureQuery query, CancellationToken token)
        {
            return _dapperRepository.WithConnectionAsync(async c =>
            {
                var sql = VersionSql;
                if (query.Version == 0)
                {
                    sql = FirstQuery;
                }
                //SELECT*
                //    FROM sb.Question AS q
                //    CROSS APPLY CHANGETABLE
                //(VERSION sb.Question , (Id), (q.Id)) AS c;

                //SELECT e.*,  
                //c.*
                //    FROM CHANGETABLE(CHANGES sb.Question, 56123) AS c
                //LEFT OUTER JOIN sb.Question AS e
                //    ON e.id = c.id


                var result = (await c.QueryAsync<T>(sql, new { version = query.Version, PageNumber = query.Page, PageSize = 50 })).ToList();

                var l = result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D");

                var max = result.Max(m => m.SYS_CHANGE_VERSION);

                return (l[false], l[true].Select(s => s.Id), max);

                //using (var grid = await c.QueryMultipleAsync(WriteSql + ";" + DeleteSql, new { version = query.Version, PageNumber = query.Page, PageSize = 50 }).ConfigureAwait(false))
                //{
                //    var write = (await grid.ReadAsync<T>().ConfigureAwait(false)).ToLookup(p => p.IsDeleted);
                //    var delete = await grid.ReadAsync<SearchWriteBaseDto>().ConfigureAwait(false);

                //    var deleteList = delete.Union(write[true]
                //        .Select(s => new SearchWriteBaseDto { Id = s.Id, Version = s.Version })).ToList();

                //    var update = write[false].ToList();
                //    long max = 0, maxDelete = 0;
                //    if (update.Count > 0)
                //    {
                //        max = update.Max(m => m.Version);
                //    }
                //    if (deleteList.Count > 0)
                //    {
                //        maxDelete = deleteList.Max(m => m.Version);
                //    }
                //    return (update.AsEnumerable(), deleteList.AsEnumerable(), new[] { max, maxDelete }.Max());
                //}
            }, token);
        }
    }
}