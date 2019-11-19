using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Query.Query.Sync;
using Cloudents.Query.Stuff;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.SearchSync
{
    public abstract class SyncAzureSearchQueryHandler<T> where T : new()
    {
        protected abstract string VersionSql { get; }

        protected abstract string FirstQuery { get; }


        private readonly IStatelessSession _session;

        protected SyncAzureSearchQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        /// <summary>
        /// The page size to query default is 100
        /// </summary>
        protected virtual int PageSize => 100;


        public async Task<(IEnumerable<T> update, IEnumerable<string> delete, long version)> GetAsync(SyncAzureQuery query, CancellationToken token)
        {
            var sql = query.Version == 0 ? FirstQuery : VersionSql;

            var sqlQuery = _session.CreateSQLQuery(sql);
            sqlQuery.SetInt32("PageSize", PageSize);
            sqlQuery.SetInt32("PageNumber", query.Page);
            if (sqlQuery.NamedParameters.Any(a => string.Equals(a, "Version", StringComparison.OrdinalIgnoreCase)))
            {
                sqlQuery.SetInt64("Version", query.Version);
            }
            sqlQuery.SetResultTransformer(new AzureSyncBaseDtoTransformer<AzureSyncBaseDto<T>, T>());


            var result = await sqlQuery.ListAsync<AzureSyncBaseDto<T>>(token);
            if (result.Count == 0)
            {
                return (Enumerable.Empty<T>(), Enumerable.Empty<string>(), 0);
            }

            var lookupTable = SeparateUpdateFromDelete(result);

            var max = result.Max(m => m.SYS_CHANGE_VERSION.GetValueOrDefault());

            return (lookupTable[false].Select(s => s.Data), lookupTable[true].Select(s => s.Id), max);

        }

        protected virtual ILookup<bool, AzureSyncBaseDto<T>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<T>> result)
        {
            return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D");
        }
    }
}