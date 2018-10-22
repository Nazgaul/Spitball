using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Sync;

namespace Cloudents.Infrastructure.Database.Query.SearchSync
{
    public class CourseSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<CourseSearchDto>,
        IQueryHandler<SyncAzureQuery, (IEnumerable<CourseSearchDto> update, IEnumerable<long> delete, long version)>
    {
        private readonly FluentQueryBuilder _queryBuilder;
        public CourseSyncAzureSearchQueryHandler(ReadonlyStatelessSession session, FluentQueryBuilder queryBuilder) : base(session)
        {
            _queryBuilder = queryBuilder;
        }
        protected override int PageSize => 500;

        protected override FluentQueryBuilder VersionSql
        {
            get
            {
                var qb = _queryBuilder;

                qb.InitTable<Course>();
                qb.CustomTable(
                    $"right outer join CHANGETABLE (CHANGES {qb.Table<Course>()}," +
                    $" {qb.Param("Version")}) AS c ON {qb.ColumnAlias<Course>(q => q.Name)} = c.Name");
                SimilarQuery(qb);
                return qb;
            }
        }
        protected override FluentQueryBuilder FirstQuery
        {
            get
            {
                var qb = _queryBuilder;
                qb.InitTable<Course>()
                    
                    .CustomTable(
                        $"CROSS APPLY CHANGETABLE (VERSION {qb.Table<Course>()}," +
                        $" (Name), ({qb.ColumnAlias<Course>(x => x.Name)})) AS c"
                    );
                SimilarQuery(qb);
                return qb;
            }
        }


        private static void SimilarQuery(FluentQueryBuilder qb)
        {
            //qb.Join<Course, University>(c => c.University, u => u.Id);
            qb
                .Select<Course>(s => s.Name, nameof(CourseSearchDto.Name))
                .Select("c.*")
                .AddOrder<Course>(o => o.Name)
                .Paging("PageSize", "PageNumber");


        }
    }
}