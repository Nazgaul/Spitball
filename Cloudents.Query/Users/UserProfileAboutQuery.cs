using System.Linq;
using Cloudents.Core.DTOs;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using Cloudents.Core.Interfaces;
using Cloudents.Core.DTOs.Users;

namespace Cloudents.Query.Users
{
    public class UserProfileAboutQuery : IQuery<UserProfileAboutDto>
    {
        public UserProfileAboutQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }



        internal sealed class UserProfileAboutQueryHandler : IQueryHandler<UserProfileAboutQuery, UserProfileAboutDto>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly IUrlBuilder _urlBuilder;

            public UserProfileAboutQueryHandler(QuerySession querySession, IUrlBuilder urlBuilder)
            {
                _urlBuilder = urlBuilder;
                _statelessSession = querySession.StatelessSession;
            }

            public async Task<UserProfileAboutDto> GetAsync(UserProfileAboutQuery query, CancellationToken token)
            {
                var result = _statelessSession.Query<TutorReview>()
                       .Fetch(f => f.User)
                       .Where(w => w.Tutor.Id == query.UserId)
                       .Where(w => w.Review != null && w.Review != string.Empty)
                       .OrderByDescending(x=>x.DateTime)
                       .Select(s => new TutorReviewDto()
                       {
                           Id = s.User.Id,
                           Image = s.User.ImageName,
                           ReviewText = s.Review,
                           Rate = s.Rate,
                           Created = s.DateTime,
                           Name = s.User.Name
                       })
                       .ToFuture();

                var rates = _statelessSession.Query<TutorReview>()
                    .Where(w => w.Tutor.Id == query.UserId)
                    .GroupBy(g => Math.Floor(g.Rate))
                    .Select(s => new RatesDto()
                    {
                        Rate = (int)Math.Floor(s.Key),
                        Users = s.Count()
                    }).ToFuture();

                var reviews = await result.GetEnumerableAsync(token);

                return new UserProfileAboutDto()
                {
                    Reviews = reviews.Select(s =>
                    {
                        s.Image = _urlBuilder.BuildUserImageEndpoint(s.Id, s.Image);
                        return s;
                    }),
                    Rates = await rates.GetEnumerableAsync(token)
                };
                //                using (var conn = _repository.OpenConnection())
                //                {
                //                    using (var grid = await conn.QueryMultipleAsync(@"
                //select  CourseId as Name from sb.UsersCourses uc
                //where UserId = @id
                //and ( 1 = case when exists (  select * from sb.Tutor t where t.Id = uc.UserId) 
                //	then   uc.canTeach
                //	else  1
                //end);

                //select t.bio
                //from sb.Tutor t
                //where t.Id = @id;

                //select tr.Review as ReviewText, tr.Rate, tr.DateTime as Created, u.Name, u.Image, u.Score,u.Id
                //from sb.Tutor t
                //join sb.TutorReview tr
                //	on tr.TutorId = t.Id
                //join sb.[user] u
                //	on tr.UserId = U.Id
                //where t.Id = @id", new { id = query.UserId }))
                //                    {
                //                        var retVal = new UserProfileAboutDto
                //                        {
                //                            Courses = await grid.ReadAsync<CourseDto>(),
                //                            Bio = await grid.ReadSingleOrDefaultAsync<string>(),
                //                            Reviews = await grid.ReadAsync<TutorReviewDto>()
                //                        };

                //                        return retVal;
                //                    }
                //                }
            }
        }
    }
}