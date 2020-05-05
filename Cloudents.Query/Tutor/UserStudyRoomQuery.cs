using System;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Stuff;

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

            public UserStudyRoomQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<UserStudyRoomDto>> GetAsync(UserStudyRoomQuery query, CancellationToken token)
            {
                StudyRoom? studyRoomAlias = null!;
                StudyRoomUser? studyRoomUser = null!;
                UserStudyRoomDto? resultAlias = null!;

                //TODO we can do it in linq
                var detachedQuery = QueryOver.Of(()=> studyRoomAlias)
                    .Left.JoinAlias(x => x.Users, () => studyRoomUser)
                    .Where(() => studyRoomUser.User.Id == query.UserId || studyRoomAlias.Tutor.Id == query.UserId)
                    .Select(s => s.Id);

                return await _session.QueryOver(() => studyRoomAlias)
                    .WithSubquery.WhereProperty(x => x.Id).In(detachedQuery)
                    .Where(w=>w.BroadcastTime.IfNull(DateTime.UtcNow.AddDays(1)) > DateTime.UtcNow)
                    .SelectList(sl =>
                                sl.Select(s => s.Id).WithAlias(() => resultAlias.Id)
                                .Select(s=>s.Name).WithAlias(() => resultAlias.Name)
                                .Select(s => s.DateTime.CreationTime).WithAlias(() => resultAlias.DateTime)
                                .Select(s=>s.StudyRoomType).WithAlias(() => resultAlias.Type)
                                .Select(s=>s.BroadcastTime).WithAlias(() => resultAlias.Scheduled)
                                .Select(s => s.Identifier).WithAlias(() => resultAlias.ConversationId)
                                .SelectSubQuery(QueryOver.Of<StudyRoomSession>()
                                    .Where(w=>w.StudyRoom.Id == studyRoomAlias.Id)
                                    .OrderBy(x=>x.Created).Desc
                                    .Select(s=>s.Created)
                                    .Take(1)
                                ).WithAlias(() => resultAlias.LastSession)
                                .SelectSubQuery(QueryOver.Of<StudyRoomUser>()
                                    .Where(w=>w.Room.Id == studyRoomAlias.Id)
                                    .ToRowCountQuery()).WithAlias(() => resultAlias.AmountOfUsers)
                                
                               
                    )
                    .OrderBy(()=> studyRoomAlias.DateTime.CreationTime).Desc
                    .TransformUsing(Transformers.AliasToBean<UserStudyRoomDto>())
                    .UnderlyingCriteria.SetComment(nameof(UserStudyRoomQuery))
                    .ListAsync<UserStudyRoomDto>(token);


            }
        }
    }
}