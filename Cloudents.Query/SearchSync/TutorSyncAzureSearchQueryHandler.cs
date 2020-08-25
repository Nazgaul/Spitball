using Cloudents.Core.DTOs.SearchSync;
using Dapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Query.SearchSync
{
    public class TutorSyncAzureSearchQuery : IQuery<SearchWrapperDto<TutorSearchDto>>

    {
        private const int PageSize = 200;
        public TutorSyncAzureSearchQuery(long version)
        {
            Version = version;
            //RowVersion = rowVersion;
        }

        protected TutorSyncAzureSearchQuery()
        {

        }

        public long Version { get; set; }
        public int Page { get; set; }


        internal sealed class TutorSyncAzureSearchQueryHandler : IQueryHandler<TutorSyncAzureSearchQuery, SearchWrapperDto<TutorSearchDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public TutorSyncAzureSearchQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<SearchWrapperDto<TutorSearchDto>> GetAsync(TutorSyncAzureSearchQuery query, CancellationToken token)
            {
                const string firstQuery = @"Select 
t.SbCountry as SbCountry ,
t.id as UserId,
t.name,
t.allCourses as Courses,
t.rate,
t.RateCount as ReviewsCount,
t.lessons as LessonsCount,
t.bio,
t.price,
t.imageName as Image,
t.rating as OverAllRating,
t.SubsidizedPrice  as SubsidizedPrice,
t.State
cTable.*
from sb.ReadTutor  t
CROSS APPLY CHANGETABLE(VERSION sb.readTutor, (Id), (t.Id)) AS cTable

order by t.id
offset @pageSize * @PageNumber rows
fetch next @pageSize Rows only";


                const string secondQuery = @" 
Select 
t.SbCountry as SbCountry ,
t.id as UserId,
t.name,
t.allCourses as Courses,
t.rate,
t.RateCount as ReviewsCount,
t.lessons as LessonsCount,
t.bio,
t.price,
t.imageName as Image,
t.rating as OverAllRating,
t.SubsidizedPrice  as SubsidizedPrice,
t.State,
cTable.* 
from sb.ReadTutor  t
right outer join CHANGETABLE (changes sb.[readTutor], @version) AS cTable ON t.Id = cTable.id
order by t.id
offset @pageSize * @PageNumber rows
fetch next @pageSize Rows only";


                using var conn = _dapperRepository.OpenConnection();
                var sql = query.Version == 0 ? firstQuery : secondQuery;
                var result = (await conn.QueryAsync<TutorSearchDto>(sql,
                    new
                    {
                        query.Version,
                        PageNumber = query.Page,
                        PageSize
                    })).ToList();



                var lookUp = result.ToLookup(x => x.SYS_CHANGE_OPERATION == "D" || x.State != ItemState.Ok);
                return new SearchWrapperDto<TutorSearchDto>()
                {
                    Update = lookUp[false],
                    Delete = lookUp[true].Select(s => s.Id.ToString()),
                    Version = result.Count > 0 ? result.Max(x => x.SYS_CHANGE_VERSION) : 0

                };
            }
        }
    }
}

