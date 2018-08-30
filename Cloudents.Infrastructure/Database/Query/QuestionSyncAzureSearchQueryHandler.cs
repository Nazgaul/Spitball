﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Infrastructure.Data;

namespace Cloudents.Infrastructure.Database.Query
{
    public class QuestionSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<QuestionAzureSyncDto>
    {

        public QuestionSyncAzureSearchQueryHandler(QueryBuilder queryBuilder, DapperRepository dapperRepository) : base(queryBuilder, dapperRepository)
        {
        }

        //public async Task<string> GetAsync(EmptyQuery query, CancellationToken token)
        //{
        //    var sql = $"select * {_queryBuilder.BuildInitVersionTable<Question>("q", "c")}";
        //    var sql2 = $"select * {_queryBuilder.BuildDiffVersionTable<Question>("q", "c",56123)}";
        //    var result = await _dapperRepository.WithConnectionAsync(f => { return f.QueryAsync(sql2); },token);
        //    return "xxx";
        //}

        protected override string VersionSql => $"select * {QueryBuilder.BuildDiffVersionTable<Question>("q", "c", 56123)}";

        protected override string FirstQuery => $"select * {QueryBuilder.BuildInitVersionTable<Question>("q", "c")}";
    }
}