using Cloudents.Core.DTOs;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class TutorListQuery : IQuery<ListWithCountDto<TutorCardDto>>
    {
        public TutorListQuery(long userId, string country, int page, int pageSize = 20)
        {
            UserId = userId;
            Country = country;
            Page = page;
            PageSize = pageSize;
        }


        private long UserId { get; }
        private string Country { get; }
        private int Page { get; }
        public int PageSize { get; set; }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, ListWithCountDto<TutorCardDto>>
        {
            private readonly IDapperRepository _dapper;

            public TutorListQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            //TODO: review query 
            public async Task<ListWithCountDto<TutorCardDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {

                var sql = @"Select distinct 1 as o, 'Tutor' as 'Type', rt.Id as UserId, rt.Name as 'Name', rt.Image as 'Image', rt.Courses, rt.Subjects, rt.Price,
rt.Rate, rt.RateCount, rt.Bio, rt.University, rt.Lessons, rt.Country, rt.SubsidizedPrice, rt.Rating
from sb.ReadTutor rt
join sb.UsersCourses uc 
	on rt.Id = uc.UserId and uc.CanTeach = 1
where (rt.Country = @country or @country is null)
and rt.Id != @userid
and uc.CourseId in (select uc2.CourseId from sb.UsersCourses uc2 where uc2.UserId = @userid)
union
Select distinct 2 as o, 'Tutor' as 'Type', rt.Id as UserId, rt.Name as 'Name', rt.Image as 'Image', rt.Courses, rt.Subjects, rt.Price,
rt.Rate, rt.RateCount, rt.Bio, rt.University, rt.Lessons, rt.Country, rt.SubsidizedPrice, rt.Rating
from sb.ReadTutor rt
join sb.UsersCourses uc 
	on rt.Id = uc.UserId and uc.CanTeach = 1
join sb.Course c 
	on uc.CourseId = c.Name
where (rt.Country = @country or @country is null)
and rt.Id != @userid
and c.SubjectId in (select c2.SubjectId from sb.UsersCourses uc2 join sb.Course c2 on uc2.CourseId = c2.Name where uc2.UserId = @userid)
and rt.Id not in (select uc2.UserId from sb.UsersCourses uc2 where uc2.UserId = rt.Id and uc2.CanTeach = 1 and uc2.CourseId in (
					select CourseId from sb.UsersCourses uc3 where uc3.UserId = @userid
				))
union
Select distinct 3 as o, 'Tutor' as 'Type', rt.Id as UserId, rt.Name as 'Name', rt.Image as 'Image', rt.Courses, rt.Subjects, rt.Price,
rt.Rate, rt.RateCount, rt.Bio, rt.University, rt.Lessons, rt.Country, rt.SubsidizedPrice , rt.Rating
from sb.ReadTutor rt
where (rt.Country = @country or @country is null)
and rt.Id != @userid
and rt.Id not in (select uc2.UserId from sb.UsersCourses uc2 join sb.Course c2 on uc2.CourseId = c2.Name where uc2.UserId = rt.Id 
					and c2.SubjectId in 
										(
										select c3.SubjectId from sb.UsersCourses uc3 join sb.Course c3 on uc3.CourseId = c3.Name where uc3.UserId = @userid
										)
				)
and rt.Id not in (select uc2.UserId from sb.UsersCourses uc2 where uc2.UserId = rt.Id and uc2.CanTeach = 1
					and uc2.CourseId in (
										select CourseId from sb.UsersCourses uc3 where uc3.UserId = @userid
										)
				)
order by o, rt.Rating desc
OFFSET @PageSize * (@PageNumber) ROWS
FETCH NEXT @PageSize ROWS ONLY;

Select count(distinct rt.Id) 
from sb.ReadTutor rt
where (rt.Country = @country or @country is null)
and rt.Id != @userid;";
                using (var conn = _dapper.OpenConnection())
                {
                    using (var multi = conn.QueryMultiple(sql, new { query.UserId, query.Country, query.PageSize, @PageNumber = query.Page }))
                    {
                        var tutor = await multi.ReadAsync<TutorCardDto>();
                        var count = await multi.ReadFirstAsync<int>();
                        return new ListWithCountDto<TutorCardDto>()
                        {
                            Count = count,
                            Result = tutor
                        };
                    }
                }
            }
        }
    }



}
