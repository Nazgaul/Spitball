//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities;
//using NHibernate;
//using NHibernate.Linq;

//namespace Cloudents.Query.StudyRooms
//{
//    public class TailorEdCodeQuery : IQuery<TailorEdResult?>
//    {
//        public TailorEdCodeQuery(Guid studyRoomId, string code)
//        {
//            StudyRoomId = studyRoomId;
//            Code = code;
//        }

//        private Guid StudyRoomId { get; }

//        private string Code { get; }


//        internal sealed class TailorEdCodeQueryHandler : IQueryHandler<TailorEdCodeQuery, TailorEdResult?>
//        {
//            private readonly IStatelessSession _statelessSession;

//            public TailorEdCodeQueryHandler(IStatelessSession statelessSession)
//            {
//                _statelessSession = statelessSession;
//            }

//            public async Task<TailorEdResult?> GetAsync(TailorEdCodeQuery query, CancellationToken token)
//            {
//               var result = await  _statelessSession.Query<StudyRoomUser>()
//                    .Where(w => w.Room.Id == query.StudyRoomId)
//                    .Where(w => w.Room is TailorEdStudyRoom)
//                    .Where(w => w.Code == query.Code)
//                    .Select(s => new TailorEdResult
//                    {
//                        Code = s.Code!,
//                        UserId = s.User.Id
//                    }).SingleOrDefaultAsync(token);

//               if (string.Equals(result?.Code, query.Code, StringComparison.Ordinal))
//               {
//                   return result;
//               }

//               return null;


//            }
//        }
//    }
//}