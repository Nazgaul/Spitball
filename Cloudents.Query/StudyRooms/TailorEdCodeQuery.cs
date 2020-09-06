using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.StudyRooms
{
    public class TailorEdCodeQuery : IQuery<bool>
    {
        public TailorEdCodeQuery(Guid studyRoomId, string code)
        {
            StudyRoomId = studyRoomId;
            Code = code;
        }

        private Guid StudyRoomId { get; }

        private string Code { get; }


        internal sealed class TailorEdCodeQueryHandler : IQueryHandler<TailorEdCodeQuery, bool>
        {
            private readonly IStatelessSession _statelessSession;

            public TailorEdCodeQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            public Task<bool> GetAsync(TailorEdCodeQuery query, CancellationToken token)
            {
                return _statelessSession.Query<TailorEdStudyRoom>()
                    .Where(w => w.Id == query.StudyRoomId)
                    .Where(w => w.Users.Any(a => a.Code == query.Code))
                    .AnyAsync(token);
            }
        }
    }
}