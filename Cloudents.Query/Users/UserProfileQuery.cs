﻿using NHibernate;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate.Linq;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Enum;

namespace Cloudents.Query.Users
{
    public class UserProfileQuery : IQuery<UserProfileDto?>
    {
        public UserProfileQuery(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private long Id { get; }
        private long UserId { get; }



        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
        internal sealed class UserProfileQueryHandler : IQueryHandler<UserProfileQuery, UserProfileDto?>
        {

            private readonly IStatelessSession _session;
            private readonly IUrlBuilder _urlBuilder;

            public UserProfileQueryHandler(IStatelessSession session, IUrlBuilder urlBuilder)
            {
                _urlBuilder = urlBuilder;
                _session = session;
            }

            public async Task<UserProfileDto?> GetAsync(UserProfileQuery query, CancellationToken token)
            {
                var tutorFuture = _session.Query<Core.Entities.Tutor>()
                    .Fetch(f => f.User)
                    .Where(w => w.Id == query.Id)
                    .Select(s => new UserProfileDto()
                    {
                        Id = s.Id,
                        Image = s.User.ImageName,
                        CalendarShared = _session.Query<GoogleTokens>().Any(w => w.Id == query.Id.ToString()),
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Cover = s.User.CoverImage,
                        Followers = _session.Query<Follow>().Count(w => w.User.Id == query.Id),
                        TutorCountry = s.User.SbCountry,

                        SessionTaughtTicks = _session.Query<StudyRoomSession>()
                            .Where(w=>w.StudyRoom.Tutor.Id == query.Id)
                            .Sum(s2=>s2.DurationTicks!.Value),
                        ContentCount = _session.Query<Document>()
                                           .Count(w => w.Status.State == ItemState.Ok && w.User.Id == query.Id),
                                     
                        
                        SubscriptionPrice = s.SubscriptionPrice,
                        Title = s.Title,
                        Paragraph2 = s.Paragraph2,
                        Paragraph3 = s.Paragraph3
                    }).ToFutureValue();



                //TODO
                //Lessons => Total hours
                //Students => Amount of files

                var rateQuery = _session.Query<TutorReview>()
                    .Where(w => w.Tutor.Id == query.Id)
                    .GroupBy(g=>1)
                    .Select(f =>new
                    {
                        Count = f.Count(),
                    })
                    .ToFutureValue();

                var isFollowingFuture = _session.Query<Follow>()
                    .Where(w => w.User.Id == query.Id && w.Follower.Id == query.UserId).ToFutureValue();

                var result = await tutorFuture.GetValueAsync(token);

                if (result is null)
                {
                    return null;
                }

                var isFollowing = isFollowingFuture.Value;
                result.IsFollowing = isFollowing != null;
                result.ReviewCount = rateQuery.Value.Count;


                result.IsSubscriber = isFollowing?.Subscriber ?? false;

                result.Image = _urlBuilder.BuildUserImageEndpoint(result.Id, result.Image);
                result.Cover = _urlBuilder.BuildUserImageEndpoint(result.Id, result.Cover ?? "default.jpg");
                return result;
            }
        }
    
    }
}