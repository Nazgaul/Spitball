using System.Linq;
using Cloudents.Core.DTOs;
using Dapper;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

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
        private int PageSize { get;  }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, ListWithCountDto<TutorCardDto>>
        {
            private readonly IDapperRepository _dapper;
            private readonly IUrlBuilder _urlBuilder;

            public TutorListQueryHandler(IDapperRepository dapper, IUrlBuilder urlBuilder)
            {
                _dapper = dapper;
                _urlBuilder = urlBuilder;
            }

            public async Task<ListWithCountDto<TutorCardDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
                const string sql = @"Select rt.Id as UserId,
rt.Name as 'Name', rt.ImageName as 'Image', rt.Courses, rt.Subjects, rt.Price,
rt.Rate, rt.RateCount as ReviewsCount, rt.Bio, rt.University, rt.Lessons, rt.Country, rt.SubsidizedPrice as DiscountPrice
from sb.ReadTutor rt
where rt.Country = coalesce(@country, (select country from sb.[user] where Id = @userId))
and rt.Id != @userId
order by
CASE
   WHEN exists (
				select uc.courseId from sb.UsersCourses uc where rt.Id = uc.UserId and uc.CanTeach = 1
   INTERSECT
				select uc2.CourseId from sb.UsersCourses uc2 where uc2.UserId = @userid) THEN 2
   WHEN exists (
				select c.subjectId from sb.UsersCourses uc  join sb.Course c on uc.CourseId = c.Name
				where rt.Id = uc.UserId and uc.CanTeach = 1
   intersect
				select c2.SubjectId from sb.UsersCourses uc2 join sb.Course c2 on uc2.CourseId = c2.Name 
				where uc2.UserId = @userid
				) THEN 1
   else 0
   end  desc, rt.Rating desc
OFFSET @PageSize * (@PageNumber) ROWS
FETCH NEXT @PageSize ROWS ONLY;

Select count(distinct rt.Id) 
from sb.ReadTutor rt
where rt.Country = coalesce(@country, (select country from sb.[user] where Id = @userId))
and rt.Id != @userid;";
                using (var conn = _dapper.OpenConnection())
                using (var multi = conn.QueryMultiple(sql, new { query.UserId, query.Country, query.PageSize, @PageNumber = query.Page }))
                {
                    var tutor = await multi.ReadAsync<TutorCardDto>();
                    var count = await multi.ReadFirstAsync<int>();
                    return new ListWithCountDto<TutorCardDto>()
                    {
                        Count = count,
                        Result = tutor.Select(s =>
                        {
                            s.Image = _urlBuilder.BuildUserImageEndpoint(s.UserId,s.Image);
                            return s;
                        })
                    };
                }
            }
        }
    }



}
