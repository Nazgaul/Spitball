using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Query;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Query.SearchSync
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
            var lookupTable = SeparateUpdateFromDelete(result);

            var max = result.Max(m => m.SYS_CHANGE_VERSION);

            return (lookupTable[false].Select(s => s.Data), lookupTable[true].Select(s => s.Id), max);
            
        }

        protected virtual ILookup<bool, AzureSyncBaseDto<T>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<T>> result)
        {
            return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D");
        }
    }
}