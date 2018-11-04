using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Sync;
using System.Collections.Generic;

namespace Cloudents.Infrastructure.Database.Query.SearchSync
{
    public class DocumentSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<QuestionSearchDto>,
    IQueryHandler<SyncAzureQuery, (IEnumerable<QuestionSearchDto> update, IEnumerable<string> delete, long version)>
    {
        private readonly FluentQueryBuilder _queryBuilder;
        public DocumentSyncAzureSearchQueryHandler(ReadonlyStatelessSession session, FluentQueryBuilder queryBuilder) : base(session)
        {
            _queryBuilder = queryBuilder;
        }

        protected override FluentQueryBuilder VersionSql
        {
            get
            {
                var qb = _queryBuilder;
                qb.InitTable<Document>();
                qb.CustomTable(
                    $"right outer join CHANGETABLE (CHANGES {qb.Table<Document>()}, {qb.Param("Version")}) AS c ON {qb.ColumnAlias<Document>(q => q.Id)} = c.id");
                SimilarQuery(qb);
                return qb;
            }
        }

        private void SimilarQuery(FluentQueryBuilder qb)
        {
            qb.Select<Document>(x => x.Id, nameof(DocumentSearchDto.Id));
            qb.Select<Document>(x => x.Name, nameof(DocumentSearchDto.Name));
            //TODO: tags
            qb.Select<Document>(x => x.Course, nameof(DocumentSearchDto.Course));
            qb.Select<Document>(x => x.TimeStamp.CreationTime, nameof(DocumentSearchDto.DateTime));
            qb.Join<Document, User>(q => q.User, u => u.Id);

            qb.Select<User>(x => x.Country, nameof(DocumentSearchDto.Country));
            qb.Select<User>(x => x.University, nameof(DocumentSearchDto.University));
            qb.Select("c.*")
                .AddOrder<Document>(q => q.Id)
                .Paging("PageSize", "PageNumber");
        }

        protected override FluentQueryBuilder FirstQuery
        {
            get
            {
                var qb = _queryBuilder;
                qb.InitTable<Document>();
                qb.CustomTable(
                    $"CROSS APPLY CHANGETABLE (VERSION {qb.Table<Document>()}, (Id), ({qb.Column<Document>(x => x.Id)})) AS c");
                SimilarQuery(qb);
                return qb;
            }
        }

        protected override int PageSize => 200;
    }
}