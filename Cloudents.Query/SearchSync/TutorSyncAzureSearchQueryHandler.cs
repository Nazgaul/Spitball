using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Dapper;

namespace Cloudents.Query.SearchSync
{
    public class TutorSyncAzureSearchQuery : IQuery<SearchWrapperDto<TutorSearchDto>>

    {
        public const int PageSize = 200;
        public TutorSyncAzureSearchQuery(long version, byte[] rowVersion)
        {
            Version = version;
            RowVersion = rowVersion;
        }

        protected TutorSyncAzureSearchQuery()
        {

        }


        // ReSharper disable once MemberCanBePrivate.Global Need for serialization
        public byte[] RowVersion { get;  set; }

        // ReSharper disable once MemberCanBePrivate.Global Need for serialization
        public long Version { get;  set; }

        // ReSharper disable once MemberCanBePrivate.Global Need for serialization
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
                const string firstQuery = @"with cte as(
select t.TutorId as Id,  avg(Rate) as rate,count(*) as rateCount from sb.TutorReview t
group by t.TutorId
)


Select
u.id as UserId,
u.Name,
u.Country,
t.Bio,
u.Image as Image,
T.Price,
t.State,
cte.rate as Rate,
cte.rateCount as ReviewsCount,
 (
      SELECT Max(v)
       FROM(VALUES (Max(t.version)),(Max(u.Version)),(Max(tc.Version)),(Max(c.Version)),(Max(cs.Version)),
	   (Max(tr.Version))) AS value(v)
   ) as version,
cTable.SYS_CHANGE_VERSION,
null as SYS_CHANGE_OPERATION,
cTable.Id,

(select dt.CourseId as CourseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1 for json path) as Courses,

(select cs2.Name
            from sb.Course c2
            join sb.UsersCourses uc2
                on c2.Name = uc2.CourseId
            join sb.CourseSubject cs2
                on cs2.Id = c2.SubjectId
            where uc2.UserId = u.Id and CanTeach = 1
            group by cs2.Name for json path)  as Subjects,
(select Name from sb.University where Id = u.UniversityId2) as University,
sr.lessonsCount
from sb.tutor t
CROSS APPLY CHANGETABLE(VERSION sb.[Tutor], (Id), (t.Id)) AS cTable
join sb.[user] u on t.Id = u.Id
left join cte on t.Id = cte.Id
left join sb.UsersCourses tc on u.id = tc.UserId and tc.CanTeach = 1
left join sb.Course c on tc.CourseId = c.Name
left join sb.CourseSubject cs on c.SubjectId = cs.Id
left join sb.TutorReview tr on t.Id = tr.TutorId
cross apply (
select count(*) as lessonsCount
from sb.StudyRoomSession srs
join sb.StudyRoom sr on srs.StudyRoomId  = sr.id  and srs.Duration > 6000000000 and sr.TutorId = t.id
) as sr

group by u.id,
u.Name,
u.Country,
t.Bio,
u.Image,
T.Price,
t.State,
cte.rate,
cte.rateCount,
cTable.SYS_CHANGE_VERSION,
cTable.Id,
u.UniversityId2,
sr.lessonsCount
order by version
offset @pageSize * @PageNumber rows
fetch next @pageSize Rows only";


                const string secondQuery = @"with cte as(
select t.TutorId as Id,  avg(Rate) as rate,count(*) as rateCount from sb.TutorReview t
group by t.TutorId
)


Select
u.id as UserId,
u.Name,
u.Country,
t.Bio,
u.Image as Image,
T.Price,
t.State,
cte.rate as Rate,
cte.rateCount as ReviewsCount,
 (
      SELECT Max(v)
       FROM(VALUES (Max(t.version)),(Max(u.Version)),(Max(tc.Version)),(Max(c.Version)),(Max(cs.Version)),
	   (Max(tr.Version))) AS value(v)
   ) as version,
cTable.SYS_CHANGE_VERSION,
cTable.SYS_CHANGE_OPERATION,
cTable.Id,

(select dt.CourseId as CourseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1 for json path) as Courses,

(select cs2.Name
            from sb.Course c2
            join sb.UsersCourses uc2
                on c2.Name = uc2.CourseId
            join sb.CourseSubject cs2
                on cs2.Id = c2.SubjectId
            where uc2.UserId = u.Id and CanTeach = 1
            group by cs2.Name for json path)  as Subjects,
(select Name from sb.University where Id = u.UniversityId2) as University,
sr.lessonsCount
from sb.tutor t
full outer join CHANGETABLE (changes sb.[Tutor], @version) AS cTable ON t.Id = cTable.id
left join sb.[user] u on t.Id = u.Id
left join cte on t.Id = cte.Id
left join sb.UsersCourses tc on u.id = tc.UserId and tc.CanTeach = 1
left join sb.Course c on tc.CourseId = c.Name
left join sb.CourseSubject cs on c.SubjectId = cs.Id
left join sb.TutorReview tr on t.Id = tr.TutorId
cross apply (
select count(*) as lessonsCount
from sb.StudyRoomSession srs
join sb.StudyRoom sr on srs.StudyRoomId  = sr.id  and srs.Duration > 6000000000 and sr.TutorId = t.id
) as sr

group by u.id,
u.Name,
u.Country,
t.Bio,
u.Image,
T.Price,
t.State,
cte.rate,
cte.rateCount,
cTable.SYS_CHANGE_VERSION,
cTable.SYS_CHANGE_OPERATION,
cTable.Id,
u.UniversityId2,
sr.lessonsCount
having (
      SELECT Max(v)
       FROM (VALUES (Max(t.version)),(Max(u.Version)),(Max(tc.Version)),(Max(c.Version)),(Max(cs.Version)),
	   (Max(tr.Version))) AS value(v)
   ) > @RowVersion or
 (
      SELECT Max(v)
       FROM (VALUES (Max(t.version)),(Max(u.Version)),(Max(tc.Version)),(Max(c.Version)),(Max(cs.Version)),
	   (Max(tr.Version))) AS value(v)
   ) is null
order by version
offset @pageSize * @PageNumber rows
fetch next @pageSize Rows only";


              

                using (var conn = _dapperRepository.OpenConnection())
                {
                    var sql = query.Version == 0 ? firstQuery : secondQuery;
                    var result = (await conn.QueryAsync<TutorSearchDto>(sql,
                         new
                         {
                             query.Version,
                             RowVersion =  query.RowVersion ?? new[] { Convert.ToByte(0) },
                             PageNumber = query.Page,
                             PageSize
                         })).ToList();



                    var lookUp = result.ToLookup(x => x.SYS_CHANGE_OPERATION == "D" || x.State != ItemState.Ok);
                    return new SearchWrapperDto<TutorSearchDto>()
                    {
                        Update = lookUp[false],
                        Delete = lookUp[true].Select(s => s.Id.ToString()),
                        Version = result.Count > 0 ? result.Max(x=>x.SYS_CHANGE_VERSION) : 0

                    };
                }
            }
        }
    }
}

