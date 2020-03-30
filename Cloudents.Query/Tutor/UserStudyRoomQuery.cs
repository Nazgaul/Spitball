using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
                StudyRoomUser? studyRoomUserAlias = null;
                User? userAlias = null;

                UserStudyRoomDto? resultAlias = null;


                

                var detachedQuery = QueryOver.Of<StudyRoomUser>()
                    .Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Room.Id);





                return await _session.QueryOver(() => studyRoomAlias)
                    .JoinAlias(x => x.Users, () => studyRoomUserAlias)
                    .JoinEntityAlias(() => userAlias,
                        () => userAlias.Id == studyRoomUserAlias.User.Id && userAlias.Id != query.UserId)
                    .WithSubquery.WhereProperty(x => x.Id).In(detachedQuery)
                    .SelectList(sl =>
                            sl.Select(s => userAlias.Name).WithAlias(() => resultAlias.Name)
                                .Select(s => userAlias.ImageName).WithAlias(() => resultAlias.Image)
                                .Select(s => userAlias.Online).WithAlias(() => resultAlias.Online)
                                .Select(s => userAlias.Id).WithAlias(() => resultAlias.UserId)
                                .Select(s => s.Id).WithAlias(() => resultAlias.Id)
                                .Select(s => s.DateTime.CreationTime).WithAlias(() => resultAlias.DateTime)
                                .Select(s => s.Identifier).WithAlias(() => resultAlias.ConversationId)
                                .Select(Projections.SqlFunction("coalesce", NHibernateUtil.DateTime,
                                                    Projections.Property(() => studyRoomAlias.DateTime.UpdateTime),
                                                    Projections.Property(() => studyRoomAlias.DateTime.CreationTime)))
                                    .WithAlias(() => resultAlias.LastSession)
                    ) 
                    .OrderBy(Projections.SqlFunction("COALESCE",NHibernateUtil.Object,
                        Projections.Property(() => studyRoomAlias.DateTime.UpdateTime),
                        Projections.Property(() => studyRoomAlias.DateTime.CreationTime))).Desc
                    //TODO on nhibernate 5.3 need to fix.
                    .TransformUsing(Transformers.AliasToBean<UserStudyRoomDto>())
                    .ListAsync<UserStudyRoomDto>(token);/*.OrderByDescending(o => o.LastActive > o.DateTime ? o.LastActive : o.DateTime)*/


            }
        }
    }
}