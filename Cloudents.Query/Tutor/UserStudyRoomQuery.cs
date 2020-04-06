using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class UserStudyRoomQuery : IQuery<IEnumerable<UserStudyRoomDto>>
    {
        public UserStudyRoomQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

        internal sealed class UserStudyRoomQueryHandler : IQueryHandler<UserStudyRoomQuery, IEnumerable<UserStudyRoomDto>>
        {
            private readonly IStatelessSession _session;

            public UserStudyRoomQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<UserStudyRoomDto>> GetAsync(UserStudyRoomQuery query, CancellationToken token)
            {
                StudyRoom? studyRoomAlias = null;
                UserStudyRoomDto? resultAlias = null;

                //TODO we can do it in linq
                var detachedQuery = QueryOver.Of<StudyRoomUser>()
                    .Left.JoinAlias(x => x.Room, () => studyRoomAlias)
                    .Where(w => w.User.Id == query.UserId || studyRoomAlias.Tutor.Id == query.UserId)
                    .Select(s => s.Room.Id);

                return await _session.QueryOver(() => studyRoomAlias)
                    .WithSubquery.WhereProperty(x => x.Id).In(detachedQuery)
                    .SelectList(sl =>
                                sl.Select(s => s!.Id).WithAlias(() => resultAlias!.Id)
                                .Select(s=>s!.Name).WithAlias(() => resultAlias!.Name)
                                .Select(s => s!.DateTime.CreationTime).WithAlias(() => resultAlias!.DateTime)
                                .Select(s => s!.Identifier).WithAlias(() => resultAlias!.ConversationId)
                                    .SelectSubQuery(QueryOver.Of<StudyRoomSession>()
                                        .Where(w=>w.StudyRoom.Id == studyRoomAlias.Id)
                                        .OrderBy(x=>x.Created).Desc
                                        .Select(s=>s.Created)
                                        .Take(1)
                                    ).WithAlias(() => resultAlias!.LastSession)
                               
                    )
                    .OrderBy(Projections.SqlFunction("COALESCE", NHibernateUtil.Object,
                        Projections.Property(() => studyRoomAlias!.DateTime.UpdateTime),
                        Projections.Property(() => studyRoomAlias!.DateTime.CreationTime))).Desc
                    //TODO on nhibernate 5.3 need to fix.
                    .TransformUsing(Transformers.AliasToBean<UserStudyRoomDto>())
                    .UnderlyingCriteria.SetComment(nameof(UserStudyRoomQuery))
                    .ListAsync<UserStudyRoomDto>(token);


            }
        }
    }
}