﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Dapper;

namespace Cloudents.Query.SearchSync
{
    public class TutorSyncAzureSearchQuery :  IQuery<SearchWrapperDto<TutorSearchDto>>

    {
        public const int PageSize = 300;
        public TutorSyncAzureSearchQuery(long version, byte[] rowVersion)
        {
            Version = version;
            RowVersion = rowVersion;
        }

        protected TutorSyncAzureSearchQuery()
        {

        }

        
        // ReSharper disable once MemberCanBePrivate.Global Need for serialization
        public byte[] RowVersion { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global Need for serialization
        public long Version { get; private set; }

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
                const string firstQuery = @"
with cte as(
select t.TutorId as Id,  avg(Rate) as rate,count(*) as rateCount from sb.TutorReview t
group by t.TutorId
)

Select  
u.id as UserId,
u.Name,
u.Country,
t.Bio,
u.Image,
T.Price,
t.State,
cte.rate as Rate,
cte.rateCount as ReviewsCount,
 (
      SELECT Max(v) 
       FROM (VALUES (t.version),(u.Version),(tc.Version),(c.Version),(cs.Version),(tr.Version)) AS value(v) 
   ) as version,
cTable.*,
c.Name as CourseName,
cs.Name as CourseSubject
from sb.tutor t
CROSS APPLY CHANGETABLE (VERSION sb.[Tutor], (Id), (t.Id)) AS cTable
join sb.[user] u on t.Id = u.Id
left join cte on t.Id = cte.Id
left join sb.UsersCourses tc on u.id = tc.UserId and tc.CanTeach = 1
left join sb.Course c on tc.CourseId = c.Name
left join sb.CourseSubject cs on c.SubjectId = cs.Id
left join sb.TutorReview tr on t.Id = tr.TutorId
";

                const string versionQuery = @"
with cte as(
select t.TutorId as Id,  avg(Rate) as rate,count(*) as rateCount from sb.TutorReview t
group by t.TutorId
)

Select  
COALESCE(u.id,cTable.id ) as UserId,
u.Name,
u.Country,
t.Bio,
u.Image,
T.Price, 
cte.rate as Rate,
cte.rateCount as ReviewsCount,
 (
      SELECT Max(v) 
       FROM (VALUES (t.version),(u.Version),(tc.Version),(c.Version),(cs.Version),(tr.Version)) AS value(v) 
   ) as version,
cTable.*,
c.Name as CourseName,
cs.Name as CourseSubject
from sb.tutor t
full join CHANGETABLE (changes sb.[Tutor], @version) AS cTable ON t.Id = cTable.id 
left join sb.[user] u on t.Id = u.Id
left join cte on t.Id = cte.Id
left join sb.UsersCourses tc on u.id = tc.UserId and tc.CanTeach = 1
left join sb.Course c on tc.CourseId = c.Name
left join sb.CourseSubject cs on c.SubjectId = cs.Id
left join sb.TutorReview tr on t.Id = tr.TutorId
where (t.version > @RowVersion  or (t.Version is null))

or u.Version > @RowVersion
or tc.Version > @RowVersion
or c.Version > @RowVersion
or cs.Version > @RowVersion
or tr.Version > @RowVersion";

                var sql = query.Version == 0 ? firstQuery : versionQuery;


                long version = 0;
                


                using (var conn = _dapperRepository.OpenConnection())
                {
                    var orderDictionary = new Dictionary<long, TutorSearchDto>();
                    var result = await conn.QueryAsync<TutorSearchDto, string, string, TutorSearchDto>(sql,
                        (dto, course, subject) =>
                        {
                            version = Math.Max(dto.SYS_CHANGE_VERSION, version);
                            if (!orderDictionary.TryGetValue(dto.UserId, out var tutorEntry))
                            {
                                tutorEntry = dto;
                                tutorEntry.Courses = new List<string>();
                                tutorEntry.Subjects = new List<string>();
                                orderDictionary.Add(tutorEntry.UserId, tutorEntry);
                            }

                            if (course != null)
                            {
                                tutorEntry.Courses.Add(course);
                            }

                            if (subject != null)
                            {
                                tutorEntry.Subjects.Add(subject);
                            }

                            return tutorEntry;
                        }, new
                        {
                            query.Version,
                            query.RowVersion,
                            query.Page
                        }, splitOn: "CourseName,CourseSubject");


                    var retVal = result.Distinct();

                    var lookUp = retVal.ToLookup(x => x.SYS_CHANGE_OPERATION == "D" || x.State != ItemState.Ok);
                    return new SearchWrapperDto<TutorSearchDto>()
                    {
                        Update = lookUp[false],
                        Delete = lookUp[true].Select(s=>s.UserId.ToString()),
                        Version = version

                    };
                }
            }
        }
    }
}