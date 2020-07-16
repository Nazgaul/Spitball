using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class SeoStudyRoomQuery : IQuery<StudyRoomSeoDto?>
    {
        public SeoStudyRoomQuery(Guid id)
        {
            Id = id;
        }

        private Guid Id { get; }


        internal sealed class StudyRoomQueryHandler : IQueryHandler<SeoStudyRoomQuery, StudyRoomSeoDto?>
        {
            private readonly IStatelessSession _statelessSession;

            public StudyRoomQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            public Task<StudyRoomSeoDto?> GetAsync(SeoStudyRoomQuery query, CancellationToken token)
            {
              return _statelessSession.Query<StudyRoom>()
                    .Where(w => w.Id == query.Id)
                    .Select(s => new StudyRoomSeoDto
                    {
                        Description = ((BroadCastStudyRoom) s).Description,
                        Name = s.Name,
                        TutorName = s.Tutor.User.Name
                    }).FirstOrDefaultAsync(token);
            }
        }
    }
}