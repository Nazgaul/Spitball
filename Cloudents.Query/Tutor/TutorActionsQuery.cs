using Cloudents.Core.DTOs.Tutors;
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
        public TutorActionsQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

        internal sealed class TutorActionsQueryHandler : IQueryHandler<TutorActionsQuery, TutorActionsDto>
        {

            private readonly IStatelessSession _session;
            public TutorActionsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<TutorActionsDto> GetAsync(TutorActionsQuery query, CancellationToken token)
            {
                var calendarFuture = _session.Query<GoogleTokens>()
                    .WithOptions(w => w.SetComment(nameof(TutorActionsQuery)))
                    .Where(w => w.Id == query.UserId.ToString())
                    .ToFutureValue(f => f.Any());

                var hoursFuture = _session.Query<TutorHours>()
                    .Where(w => w.Tutor.Id == query.UserId)
                    .ToFutureValue(f => f.Any());



                var bookedSessionFuture = _session.Query<StudyRoomUser>()
                    .Fetch(f => f.Room)
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => _session.Query<AdminTutor>()
                        .Select(s => s.Tutor.Id).Contains(w.Room.Tutor.Id))
                    .Take(1)
                    .ToFutureValue(f => f.Any());

                var userDetailsFuture = _session.Query<User>()
                    .Fetch(f => f.Tutor)
                    .Where(w => w.Id == query.UserId)
                    .Select(s => new
                    {
                        s.PhoneNumberConfirmed,
                        s.EmailConfirmed,
                        s.Description,
                        s.Tutor!.Bio,
                        s.CoverImage
                    }).ToFutureValue();

                var coursesFuture = _session.Query<UserCourse>()
                    .Where(w => w.User.Id == query.UserId)
                    .ToFutureValue(f => f.Any());

                var liveSessionFuture = _session.Query<BroadCastStudyRoom>()
                     .Where(w => w.Tutor.Id == query.UserId)
                     .ToFutureValue(f => f.Any());

                var documentFuture = _session.Query<Document>()
                    .Where(w => w.User.Id == query.UserId)
                    .ToFutureValue(f => f.Any());





                var calendarShared = await calendarFuture.GetValueAsync(token);
                var haveHours = await hoursFuture.GetValueAsync(token);
                var bookedSession = await bookedSessionFuture.GetValueAsync(token);
                var userDetails = userDetailsFuture.Value;

                var res = new TutorActionsDto()
                {
                    CalendarShared = calendarShared,
                    HaveHours = haveHours,
                    PhoneVerified = userDetails.PhoneNumberConfirmed,
                    EmailVerified = userDetails.EmailConfirmed,
                    Courses = coursesFuture.Value,
                    LiveSession = liveSessionFuture.Value,
                    UploadContent = documentFuture.Value,
                    EditProfile = userDetails.Description != null || userDetails.Bio != null || userDetails.CoverImage != null
                };

                return res;
            }
        }
    }
}
