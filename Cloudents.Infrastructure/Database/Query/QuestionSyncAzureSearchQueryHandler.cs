using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Data;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<QuestionAzureSyncDto>,
        IQueryHandler<SyncAzureQuery, (IEnumerable<QuestionAzureSyncDto> update, IEnumerable<long> delete, long version)>
    {

        public QuestionSyncAzureSearchQueryHandler(QueryBuilder queryBuilder, DapperRepository dapperRepository) : base(queryBuilder, dapperRepository)
        {
        }

       

        protected override string VersionSql => $"select * {QueryBuilder.BuildDiffVersionTable<Question>("q", "c", 56123)}";

        protected override string FirstQuery => $"select * {QueryBuilder.BuildInitVersionTable<Question>("q", "c")}";
    }
}