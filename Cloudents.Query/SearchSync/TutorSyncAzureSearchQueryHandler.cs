﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Query.Query.Sync;
using Dapper;

namespace Cloudents.Query.SearchSync
{
    public class TutorSyncAzureSearchQuery : SyncAzureQuery, IQuery<SearchWrapperDto<TutorSearchDto>>
        
    {
        public TutorSyncAzureSearchQuery(long version, int page, byte[] rowVersion) : base(version, page)
        {
            RowVersion = rowVersion;
        }

        private byte[] RowVersion { get; }


        internal sealed class TutorSyncAzureSearchQueryHandler: IQueryHandler<TutorSyncAzureSearchQuery, SearchWrapperDto<TutorSearchDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorSyncAzureSearchQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<SearchWrapperDto<TutorSearchDto>> GetAsync(TutorSyncAzureSearchQuery query, CancellationToken token)
            {
                const string FirstQuery = @"
with cte as(
select t.TutorId as Id,  avg(Rate) as rate,count(*) as rateCount from sb.TutorReview t
group by t.TutorId
)

Select  
u.id as Id,
u.Name,
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
CROSS APPLY CHANGETABLE (VERSION sb.[Tutor], (Id), (t.Id)) AS cTable
join sb.[user] u on t.Id = u.Id
left join cte on t.Id = cte.Id
left join sb.UsersCourses tc on u.id = tc.UserId and tc.CanTeach = 1
left join sb.Course c on tc.CourseId = c.Name
left join sb.CourseSubject cs on c.SubjectId = cs.Id
left join sb.TutorReview tr on t.Id = tr.TutorId
";

                var sql = query.Version == 0 ? FirstQuery : FirstQuery;
                long version = 0;
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var orderDictionary = new Dictionary<long, TutorSearchDto>();
                    var result = await conn.QueryAsync<TutorSearchDto, string, string, TutorSearchDto>(sql,
                        (dto, course, subject) =>
                        {
                            version = Math.Max(dto.SYS_CHANGE_VERSION, version);
                            if (!orderDictionary.TryGetValue(dto.Id, out var tutorEntry))
                            {
                                tutorEntry = dto;
                                tutorEntry.Courses = new List<string>();
                                tutorEntry.Subjects = new List<string>();
                                orderDictionary.Add(tutorEntry.Id, tutorEntry);
                            }

                            tutorEntry.Courses.Add(course);
                            tutorEntry.Subjects.Add(subject);
                            return tutorEntry;
                        }, new
                        {
                            query.Version,
                            query.RowVersion
                        }, splitOn: "CourseName,CourseSubject");
                    return new SearchWrapperDto<TutorSearchDto>()
                    {
                        Update = result.Distinct(),
                        Delete = null,
                        Version = version

                    };
                }
            }
        }
    }
}