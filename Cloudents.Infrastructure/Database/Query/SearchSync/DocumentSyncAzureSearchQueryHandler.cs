using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Sync;
using System.Collections.Generic;

namespace Cloudents.Infrastructure.Database.Query.SearchSync
{
    public class DocumentSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<DocumentSearchDto>,
    IQueryHandler<SyncAzureQuery, (IEnumerable<DocumentSearchDto> update, IEnumerable<string> delete, long version)>
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
            qb.Select<Document>(x => x.Id, nameof(DocumentSearchDto.ItemId));
            qb.Select<Document>(x => x.Name, nameof(DocumentSearchDto.Name));
            qb.Select<Document>(x => x.Course, nameof(DocumentSearchDto.Course));
            qb.Select<Document>(x => x.Language, nameof(DocumentSearchDto.Language));
            qb.Select<Document>(x => x.University, nameof(DocumentSearchDto.University));
            qb.Select<Document>(x => x.Type, nameof(DocumentSearchDto.Type));
            //TODO - we do not implement component as expression
            qb.Select(
                $"{qb.TableAlias<Document>()}.{nameof(Document.TimeStamp.CreationTime)} as {nameof(DocumentSearchDto.DateTime)}");
           // qb.Select<Document>(x => x.TimeStamp.CreationTime, nameof(DocumentSearchDto.DateTime));
            qb.Select(
                $" (select STRING_AGG(dt.TagId, ', ') FROM sb.DocumentsTags dt where {qb.ColumnAlias<Document>(x => x.Id)} = dt.DocumentId) AS {nameof(DocumentSearchDto.Tags)}");
            qb.LeftJoin<Document, University>(q => q.University.Id, u => u.Id);

            qb.Select<University>(x => x.Country, nameof(DocumentSearchDto.Country));
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