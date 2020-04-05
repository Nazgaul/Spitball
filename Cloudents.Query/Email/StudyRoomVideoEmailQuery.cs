//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Message.Email;
//using NHibernate;
//using NHibernate.Linq;

//namespace Cloudents.Query.Email
//{
//    public class StudyRoomVideoEmailQuery : IQuery<StudyRoomVideoMessage>
//    {
//        public StudyRoomVideoEmailQuery(string studyRoomSession)
//        {
//            StudyRoomSession = studyRoomSession;
//        }

//        private string StudyRoomSession { get; }

//        internal sealed class StudyRoomVideoEmailQueryHandler : IQueryHandler<StudyRoomVideoEmailQuery, StudyRoomVideoMessage>
//        {
//            private readonly IStatelessSession _statelessSession;

//            public StudyRoomVideoEmailQueryHandler(QuerySession statelessSession)
//            {
//                _statelessSession = statelessSession.StatelessSession;
//            }

//            public async Task<StudyRoomVideoMessage> GetAsync(StudyRoomVideoEmailQuery query, CancellationToken token)
//            {
//                var studyRoomSession = await _statelessSession.Query<StudyRoomSession>()
//                    .WithOptions(w => w.SetComment(nameof(StudyRoomVideoEmailQuery)))
//                    .Fetch(f=>f.StudyRoom)
//                    .ThenFetch(f=>f.Tutor)
//                    .Fetch(f=>f.StudyRoom)
//                    .ThenFetchMany(f=>f.Users)
//                    .ThenFetch(f=>f.User)
//                    .Where(w => w.SessionId == query.StudyRoomSession).SingleAsync(token);
//                var tutor = studyRoomSession.StudyRoom.Tutor;

//                var user = studyRoomSession.StudyRoom.Users.Single(w => w.User.Id != tutor.Id);


//                var message = new StudyRoomVideoMessage(
//                    tutor.User.FirstName,
//                    user.User.Name,
//                    DateTime.UtcNow,
//                    user.User.Email,
//                    user.User.Language
//                );
//                return message;
//            }
//        }
//    }
//}