using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class SessionRecordingQuery : IQuery<IEnumerable<SessionRecordingDto>>
    {
        public SessionRecordingQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class SessionRecordingQueryHandler : IQueryHandler<SessionRecordingQuery, IEnumerable<SessionRecordingDto>>
        {
            private readonly IStatelessSession _statelessSession;


            public SessionRecordingQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }

            public async Task<IEnumerable<SessionRecordingDto>> GetAsync(SessionRecordingQuery query, CancellationToken token)
            {
                SessionRecordingDto resultAlias = null;
                StudyRoomSession studyRoomSessionAlias = null;
                StudyRoom studyRoomAlias = null;
                Cloudents.Core.Entities.Tutor tutorAlias = null;
                User userAlias = null;

                var detachedQuery = QueryOver.Of<StudyRoomUser>()
                    .Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Room.Id);


                return await _statelessSession.QueryOver(() => studyRoomSessionAlias)
                    .JoinEntityAlias(() => studyRoomAlias, () => studyRoomAlias.Id == studyRoomSessionAlias.StudyRoom.Id)
                    .JoinEntityAlias(() => tutorAlias, () => studyRoomAlias.Tutor.Id == tutorAlias.Id)
                    .JoinEntityAlias(() => userAlias, () => tutorAlias.Id == userAlias.Id)
                    .WithSubquery.WhereProperty(x => x.StudyRoom.Id).In(detachedQuery)
                    .Where(w => w.VideoExists)

                    .SelectList(s =>
                            s.Select(c => c.Id).WithAlias(() => resultAlias.Id)
                            .Select(c => userAlias.Id).WithAlias(() => resultAlias.TutorId)
                            .Select(c => userAlias.Name).WithAlias(() => resultAlias.TutorName)
                            .Select(c => userAlias.Image).WithAlias(() => resultAlias.TutorImage)
                            .Select(c => c.Duration).WithAlias(() => resultAlias.Duration)
                        ).TransformUsing(Transformers.AliasToBean<SessionRecordingDto>())
                     .ListAsync<SessionRecordingDto>(token);

            }
        }
    }
}
