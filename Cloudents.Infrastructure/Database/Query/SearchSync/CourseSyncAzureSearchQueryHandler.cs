using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;

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
                    $"right outer join CHANGETABLE (CHANGES {qb.Table<Course>()}, {qb.Param("Version")}) AS c ON {qb.ColumnAlias<Course>(q => q.Id)} = c.BoxId");
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
                        $"CROSS APPLY CHANGETABLE (VERSION {qb.Table<Course>()}, (BoxId), ({qb.ColumnAlias<Course>(x => x.Id)})) AS c"
                    );
                SimilarQuery(qb);
                return qb;
            }
        }


        private static void SimilarQuery(FluentQueryBuilder qb)
        {
            qb.Join<Course, University>(c => c.University, u => u.Id);
            qb.Select<Course>(s => s.Id, nameof(CourseSearchDto.Id))
                .Select<Course>(s => s.Name, nameof(CourseSearchDto.Name))
                .Select<Course>(s => s.Code, nameof(CourseSearchDto.Code))
                .Select<Course>(s => s.IsDeleted, nameof(CourseSearchDto.IsDeleted))
                .Select<University>(s => s.Id, nameof(CourseSearchDto.UniversityId))
                .Where($"{qb.ColumnAlias<Course>(c=>c.Discriminator)} = {(int)CourseType.Academic}")
                .Where($"{qb.ColumnAlias<Course>(c=>c.PrivacySetting)} = {(int)CoursePrivacySetting.AnyoneWithUrl}")
                .Select("c.*")
                .AddOrder<Course>(o => o.Id)
                .Paging("PageSize", "PageNumber");


        }

        protected override ILookup<bool, AzureSyncBaseDto<CourseSearchDto>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<CourseSearchDto>> result)
        {
            return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D" || p.Data.IsDeleted );
        }
    }
}