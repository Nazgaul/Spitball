﻿using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class TutorActionsQuery : IQuery<TutorActionsDto>
    {
        public TutorActionsQuery(long userId, Country country)
        {
            UserId = userId;
            Country = country;
        }

        private long UserId { get; }
        private Country Country { get; }

        /// <summary>
        /// Return actions - true if the user done it
        /// </summary>
        internal sealed class TutorActionsQueryHandler : IQueryHandler<TutorActionsQuery, TutorActionsDto>
        {

            private readonly IStatelessSession _session;
            public TutorActionsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }
            public async Task<TutorActionsDto> GetAsync(TutorActionsQuery query, CancellationToken token)
            {
                var calendarFuture = _session.Query<GoogleTokens>()
                    .WithOptions(w => w.SetComment(nameof(TutorActionsQuery)))
                    .Where(w => w.Id == query.UserId.ToString())
                    .Select(s => s.Id)
                    .ToFutureValue(f => f.Any());

                var bookedSessionFuture = _session.Query<StudyRoomUser>()
                    .Fetch(f => f.Room)
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => _session.Query<AdminTutor>().Where(w2 => w2.Country == query.Country)
                        .Select(s => s.Tutor.Id).Contains(w.Room.Tutor.Id))
                    .Select(s => new
                    {
                        s.Id,

                    })
                    .Take(1)
                    .ToFutureValue();

                var adminFuture = _session.Query<AdminTutor>().Where(w2 => w2.Country == query.Country)
                    .Select(s2 => s2.Tutor.Id).ToFutureValue();

                var userDetailsFuture = _session.Query<Core.Entities.Tutor>()
                    .Fetch(f => f.User)
                    .Where(w => w.Id == query.UserId)
                    .Select(s => new
                    {
                        s.User.PhoneNumberConfirmed,
                        s.User.EmailConfirmed,
                        s.Title,
                        s.Paragraph2,
                        s.User.CoverImage,
                        s.User.SbCountry
                    }).ToFutureValue();


                var liveSessionFuture = _session.Query<Course>()
                     .Where(w => w.Tutor.Id == query.UserId)
                     .Select(s => s.Id)
                     .ToFutureValue(f => f.Any());

                var stripeConnectFuture = _session.Query<Core.Entities.Tutor>()
                    .Where(w => w.Id == query.UserId)
                    .Select(s => s.SellerKey).ToFutureValue();


                var calendarShared = await calendarFuture.GetValueAsync(token);
                
                var bookedSession = await bookedSessionFuture.GetValueAsync(token);
                var userDetails = userDetailsFuture.Value;

                var res = new TutorActionsDto()
                {
                    CalendarShared = calendarShared,
                    PhoneVerified = userDetails.PhoneNumberConfirmed,
                    EmailVerified = userDetails.EmailConfirmed,
                    LiveSession = liveSessionFuture.Value,
                    StripeAccount = stripeConnectFuture.Value != null || !(userDetails.SbCountry == Country.UnitedStates),
                    BookedSession = new BookedSession()
                    {
                        Exists = bookedSession?.Id != null,
                        _TutorId = adminFuture.Value
                    },
                    EditProfile = userDetails.Paragraph2 != null 
                                  || userDetails.Title != null 
                                  || userDetails.CoverImage != null
                };

                return res;
            }
        }
    }
}
