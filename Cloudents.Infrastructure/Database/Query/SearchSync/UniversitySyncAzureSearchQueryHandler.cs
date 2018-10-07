using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Database.Query.SearchSync
{
    public class UniversitySyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<UniversitySearchDto>,
        IQueryHandler<SyncAzureQuery, (IEnumerable<UniversitySearchDto> update, IEnumerable<long> delete, long version)>
    {
        private readonly FluentQueryBuilder _queryBuilder;
        public UniversitySyncAzureSearchQueryHandler(ReadonlyStatelessSession session, FluentQueryBuilder queryBuilder) : base(session)
        {
            _queryBuilder = queryBuilder;
        }

        protected override FluentQueryBuilder VersionSql
        {
            get
            {
                var qb = _queryBuilder;

                qb.InitTable<University>();
                qb.CustomTable(
                    $"right outer join CHANGETABLE (CHANGES {qb.Table<University>()}, {qb.Param("Version")}) AS c ON {qb.ColumnAlias<University>(q => q.Id)} = c.id");
                SimilarQuery(qb);
                return qb;
            }
        }

        protected override FluentQueryBuilder FirstQuery
        {
            get
            {
                var qb = _queryBuilder;
                qb.InitTable<University>()
                    .CustomTable(
                        $"CROSS APPLY CHANGETABLE (VERSION {qb.Table<University>()}, (Id), ({qb.ColumnAlias<University>(x => x.Id)})) AS c"
                    );
                SimilarQuery(qb);
                return qb;
            }
        }

        private static void SimilarQuery(FluentQueryBuilder qb)
        {
            qb.Select<University>(s => s.Id, nameof(UniversitySearchDto.Id))
                .Select<University>(s => s.Name, nameof(UniversitySearchDto.Name))
                //.Select<University>(s => s.Image, nameof(UniversitySearchDto.Image))
                .Select<University>(s => s.IsDeleted, nameof(UniversitySearchDto.IsDeleted))
                .Select<University>(s => s.Extra, nameof(UniversitySearchDto.Extra))
                //.Select<University>(s => s.Longitude, nameof(UniversitySearchDto.Longitude))
                //.Select<University>(s => s.Latitude, nameof(UniversitySearchDto.Latitude))
                .Select<University>(s => s.Pending, nameof(UniversitySearchDto.Pending))
                .Select<University>(s => s.Country, nameof(UniversitySearchDto.Country))
                .Select("c.*")
                .AddOrder<University>(o => o.Id)
                .Paging("PageSize", "PageNumber");
        }

        protected override ILookup<bool, AzureSyncBaseDto<UniversitySearchDto>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<UniversitySearchDto>> result)
        {
            return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D" || p.Data.IsDeleted || p.Data.Pending);
        }
    }
}