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
                    .WithOptions(w => w.SetComment(nameof(TutorReview)))
                       .Fetch(f => f.User)
                       .Where(w => w.Tutor.Id == query.UserId)
                       .Where(w => w.Review != null && w.Review != string.Empty)
                       .OrderByDescending(x => x.DateTime)
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

            }
        }
    }
}