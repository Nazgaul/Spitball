using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class UserProfileAboutQuery : IQuery<UserProfileAboutDto>
    {
        public UserProfileAboutQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; set; }



        internal sealed class UserProfileAboutQueryHandler : IQueryHandler<UserProfileAboutQuery, UserProfileAboutDto>
        {
            private readonly DapperRepository _repository;

            public UserProfileAboutQueryHandler(DapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<UserProfileAboutDto> GetAsync(UserProfileAboutQuery query, CancellationToken token)
            {
                using (var conn = _repository.OpenConnection())
                {
                    using (var grid = await conn.QueryMultipleAsync(@"
select CourseId as Name from sb.UsersCourses
where UserId = @id;

select utt.bio
from sb.UserType ut
join sb.UserTutor utt on ut.id = utt.userRole_id
where ut.userid = @id;

select tr.Review as ReviewText, tr.Rate, tr.DateTime as Created, u.Name, u.Image, u.Score
from sb.UserType ut
join sb.UserTutor utt 
	on ut.id = utt.userRole_id
join sb.TutorReview tr
	on tr.TutorId = utt.UserRole_id
join sb.[user] u
	on tr.UserId = U.Id
where ut.userid = @id", new {id = query.UserId}))
                    {
                        var retVal = new UserProfileAboutDto
                        {
                            Courses = await grid.ReadAsync<CourseDto>(),
                            Bio = await grid.ReadSingleOrDefaultAsync<string>(),
                            Reviews = await grid.ReadAsync<TutorReviewDto>()
                        };

                        return retVal;
                    }
                }
            }
        }
    }
}