using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using Cloudents.Core.Interfaces;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.DTOs.Tutors;

namespace Cloudents.Query.Users
{
    public class UserProfileReviewsQuery : IQuery<UserProfileReviewsDto>
    {
        public UserProfileReviewsQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }



        internal sealed class UserProfileAboutQueryHandler : IQueryHandler<UserProfileReviewsQuery, UserProfileReviewsDto>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly IUrlBuilder _urlBuilder;

            public UserProfileAboutQueryHandler(IStatelessSession querySession, IUrlBuilder urlBuilder)
            {
                _urlBuilder = urlBuilder;
                _statelessSession = querySession;
            }

            public async Task<UserProfileReviewsDto> GetAsync(UserProfileReviewsQuery query, CancellationToken token)
            {
                var result = _statelessSession.Query<TutorReview>()
                    .WithOptions(w => w.SetComment(nameof(TutorReview)))
                       .Where(w => w.Tutor.Id == query.UserId)
                       .OrderByDescending(x => x.DateTime)
                       .Select(s => new TutorReviewDto()
                       {
                           Id = s.User.Id,
                           Image = s.User.ImageName,
                           ReviewText = s.Review!,
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

                return new UserProfileReviewsDto()
                {
                    Reviews = reviews.Select(s =>
                    {
                        s.Image = _urlBuilder.BuildUserImageEndpoint(s.Id, s.Image);
                        return s;
                    }),
                    Rates = await rates.GetEnumerableAsync(token)
                };

            }
        }
    }
}