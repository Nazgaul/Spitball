﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class TutorListQuery : IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListQuery(long userId)
        {
            UserId = userId;
        }


        private long UserId { get; }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, IEnumerable<TutorListDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
                const string sql = @"select *  from (select U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) as courses,
T.Price, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						and  uc.CourseId in (select CourseId from sb.UsersCourses where UserId = @UserId)

union
select U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) as courses,
T.Price, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						join sb.Course c on uc.CourseId = c.Name
						and c.SubjectId in (Select subjectId  from sb.UsersCourses where UserId = @UserId)
						) t

where t.UserId <> @UserId
order by Rate desc
OFFSET 0 ROWS
FETCH NEXT 20 ROWS ONLY;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<TutorListDto>(sql, new
                    {
                        query.UserId

                    });

                    return retVal;
                }
            }
        }
    }



}