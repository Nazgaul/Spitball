using Cloudents.Core.DTOs;
using Cloudents.Core.Query;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    public abstract class SyncAzureSearchQueryHandler<T> where T : new()
    {
        protected abstract FluentQueryBuilder VersionSql { get; }

        protected abstract FluentQueryBuilder FirstQuery { get; }


        private readonly IStatelessSession _session;

        protected SyncAzureSearchQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }


        public async Task<(IEnumerable<T> update, IEnumerable<long> delete, long version)> GetAsync(SyncAzureQuery query, CancellationToken token)
        {
            var sql = query.Version == 0 ? FirstQuery : VersionSql;

            var sqlQuery = _session.CreateSQLQuery(sql);
            sqlQuery.SetInt32("PageSize", 50);
            sqlQuery.SetInt32("PageNumber", query.Page);
            if (sqlQuery.NamedParameters.Any(a => string.Equals(a, "Version", StringComparison.OrdinalIgnoreCase)))
            {
                sqlQuery.SetInt64("Version", query.Version);
            }
            sqlQuery.SetResultTransformer(new AzureSyncBaseDtoTransformer<AzureSyncBaseDto<T>, T>());


            var result = await sqlQuery.ListAsync<AzureSyncBaseDto<T>>(token);
            if (result.Count == 0)
            {
                return (Enumerable.Empty<T>(), Enumerable.Empty<long>(), 0);
            }

            //sqlQuery.
            var lookupTable = result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D");

            var max = result.Max(m => m.SYS_CHANGE_VERSION);

            return (lookupTable[false].Select(s => s.Data), lookupTable[true].Select(s => s.Id), max);
            //return _dapperRepository.WithConnectionAsync(async c =>
            //{
            //    var sql = VersionSql;
            //    if (query.Version == 0)
            //    {
            //        sql = FirstQuery;
            //    }
            //    //SELECT*
            //    //    FROM sb.Question AS q
            //    //    CROSS APPLY CHANGETABLE
            //    //(VERSION sb.Question , (Id), (q.Id)) AS c;

            //    //SELECT e.*,  
            //    //c.*
            //    //    FROM CHANGETABLE(CHANGES sb.Question, 56123) AS c
            //    //LEFT OUTER JOIN sb.Question AS e
            //    //    ON e.id = c.id
            //   // var result = (await c.QueryAsync<AzureSyncBaseDto<T>, T, AzureSyncBaseDto<T>>(sql,
            //   //         (c2, l) =>
            //   //         {
            //   //             return c2;
            //   //         }
            //   //     )
            //   //, splitOn: "TTT"
            //   //     , new {version = query.Version, PageNumber = query.Page, PageSize = 50}).Item1.ToList();

            //    var result = (await c.QueryAsync<AzureSyncBaseDto<T>>(sql, new { version = query.Version, PageNumber = query.Page, PageSize = 50 })).ToList();

            //    var lookupTable = result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D");

            //    var max = result.Max(m => m.SYS_CHANGE_VERSION);

            //    return (lookupTable[false], lookupTable[true].Select(s => s.Id), max);

            //    //using (var grid = await c.QueryMultipleAsync(WriteSql + ";" + DeleteSql, new { version = query.Version, PageNumber = query.Page, PageSize = 50 }).ConfigureAwait(false))
            //    //{
            //    //    var write = (await grid.ReadAsync<T>().ConfigureAwait(false)).ToLookup(p => p.IsDeleted);
            //    //    var delete = await grid.ReadAsync<SearchWriteBaseDto>().ConfigureAwait(false);

            //    //    var deleteList = delete.Union(write[true]
            //    //        .Select(s => new SearchWriteBaseDto { Id = s.Id, Version = s.Version })).ToList();

            //    //    var update = write[false].ToList();
            //    //    long max = 0, maxDelete = 0;
            //    //    if (update.Count > 0)
            //    //    {
            //    //        max = update.Max(m => m.Version);
            //    //    }
            //    //    if (deleteList.Count > 0)
            //    //    {
            //    //        maxDelete = deleteList.Max(m => m.Version);
            //    //    }
            //    //    return (update.AsEnumerable(), deleteList.AsEnumerable(), new[] { max, maxDelete }.Max());
            //    //}
            //}, token);
        }
    }
}