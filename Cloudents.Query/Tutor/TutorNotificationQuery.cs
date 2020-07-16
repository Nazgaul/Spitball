using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class TutorNotificationQuery : IQuery<TutorNotificationDto>
    {
        public TutorNotificationQuery(long tutorId)
        {
            TutorId = tutorId;
        }

        private long TutorId { get; }


        internal sealed class TutorNotificationQueryHandler : IQueryHandler<TutorNotificationQuery, TutorNotificationDto>
        {
            private readonly IStatelessSession _session;


            public TutorNotificationQueryHandler(IStatelessSession statelessSession)
            {
                _session = statelessSession;
            }
            public async Task<TutorNotificationDto> GetAsync(TutorNotificationQuery query, CancellationToken token)
            {
                var newPendingSessionPayment = _session.Query<StudyRoomSessionUser>()
                      .Fetch(f => f.StudyRoomPayment)
                      .Where(w => w.StudyRoomPayment.Tutor.Id == query.TutorId
                                  && w.Duration > StudyRoomSession.BillableStudyRoomSession
                                  && w.StudyRoomPayment.TutorApproveTime == null)
                      .ToFutureValue(f=>f.Count());

                var unreadMessages = _session.Query<ChatUser>().Where(w => w.User.Id == query.TutorId)
                    .ToFutureValue(f => f.Sum(s => (int?)s.Unread));

                var enrolledStudents = _session.Query<StudyRoomUser>()
                    .Fetch(f => f.Room)
                    .Where(w => w.Room.Tutor.Id == query.TutorId)
                    .Where(w => ((BroadCastStudyRoom)w.Room).BroadcastTime > DateTime.UtcNow)
                    .ToFutureValue(f => f.Count());

                var chatUserQueryable = _session.Query<ChatUser>()
                    .Where(w => w.ChatRoom.Tutor.Id == query.TutorId)
                    .Select(s => s.User.Id);


                var followersQueryable = _session.Query<Follow>()
                    .Where(w => w.User.Id == query.TutorId)
                    .Select(s => s.Follower.Id);


                var noChat = followersQueryable
                    .Where(w => !chatUserQueryable.Contains(w))
                    .ToFutureValue(f => f.Count());


                //var unAnsweredQuestion = _session.Query<Question>()
                //    .Where(w => w.Status.State == ItemState.Ok)
                //    .Where(w => followersQueryable.Contains(w.User.Id))
                //    .Where(w => w.Answers.All(a => a.User.Id != query.TutorId))
                //    .ToFutureValue(f => f.Count());

                var result = new TutorNotificationDto
                {
                    PendingPayment = await newPendingSessionPayment.GetValueAsync(token),
                    UnreadChatMessages = unreadMessages.Value ?? 0,
                    LiveClassRegisteredUser = enrolledStudents.Value,
                    FollowerNoCommunication = noChat.Value,
                    UnansweredQuestion = 0
                };

                return result;
            }
        }
    }
}
