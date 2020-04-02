using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
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
                    .Take(1)
                    .ToFutureValue();

                var hoursFuture = _session.Query<TutorHours>()
                    .Where(w => w.Tutor.Id == query.UserId)
                    .Take(1)
                    .ToFutureValue();

                //StudyRoom studyRoomAlias = null;
                //StudyRoomUser studyRoomUserAlias = null;
                //User userAlias = null;
                //Core.Entities.Tutor tutorAlias = null;
                //AdminTutor adminTutorAlias = null;

                var bookedSessionFuture = _session.Query<StudyRoomUser>()
                    .Fetch(f => f.Room)
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => _session.Query<AdminTutor>().Select(s => s.Tutor.Id).Contains(w.Room.Tutor.Id))
                    .Take(1)
                    .ToFutureValue();
                

                //var bookedSessionFuture = _session.QueryOver(() => studyRoomAlias)
                //   .JoinAlias(f => f.Users, () => studyRoomUserAlias)
                //   .JoinEntityAlias(() => userAlias, () => userAlias.Id == studyRoomUserAlias.User.Id)
                //   .JoinEntityAlias(() => tutorAlias, () => studyRoomAlias.Tutor.Id == tutorAlias.Id)
                //   .JoinEntityAlias(() => adminTutorAlias, () => tutorAlias.Id == adminTutorAlias.Tutor.Id)
                //    .Where(w => userAlias.Id == query.UserId)
                //    .Where(w => adminTutorAlias.Id != null)
                //    .Select(s => s.Id)
                //    .Take(1)
                //    .FutureValue<Guid?>();

                var calendarShared = await calendarFuture.GetValueAsync(token) != null;
                var haveHours = await hoursFuture.GetValueAsync(token) != null;
                var bookedSession = await bookedSessionFuture.GetValueAsync(token) != null;

                var res = new TutorActionsDto()
                {
                    CalendarShared = calendarShared,
                    HaveHours = haveHours,
                    BookedSession = bookedSession
                };

                return res;
            }
        }
    }
}
