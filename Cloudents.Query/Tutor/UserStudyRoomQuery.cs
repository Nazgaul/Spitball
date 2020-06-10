using System;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
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
                return await _session.Query<StudyRoom>()
                     .Where(w => w.Tutor.Id == 638 || w.Users.Any(a => a.User.Id == 638))
                     .Where(w => (((BroadCastStudyRoom)w).BroadcastTime != null ? ((BroadCastStudyRoom)w).BroadcastTime : DateTime.UtcNow.AddDays(1)) > DateTime.UtcNow)
                     .OrderByDescending(o => o.DateTime.CreationTime)
                     .Select(s => new UserStudyRoomDto(
                         s.Name,
                         s.Id,
                         s.DateTime.CreationTime,
                         s.Identifier,
                         s.DateTime.UpdateTime,
                         StudyRoomType.Broadcast,
                         (s as BroadCastStudyRoom) != null ? ((BroadCastStudyRoom)s).BroadcastTime : new DateTime?(),
                         s.Users.Select(s2 => s2.User.FirstName).ToList()))

                     .ToListAsync(token);
                //StudyRoom? studyRoomAlias = null!;
                //StudyRoomUser? studyRoomUser = null!;
                //UserStudyRoomDto? resultAlias = null!;

                ////TODO we can do it in linq
                //var detachedQuery = QueryOver.Of(()=> studyRoomAlias)
                //    .Left.JoinAlias(x => x.Users, () => studyRoomUser)
                //    .Where(() => studyRoomUser.User.Id == query.UserId || studyRoomAlias.Tutor.Id == query.UserId)
                //    .Select(s => s.Id);

                //return await _session.QueryOver(() => studyRoomAlias)
                //    .WithSubquery.WhereProperty(x => x.Id).In(detachedQuery)
                //    .Where(w=>((BroadCastStudyRoom)w).BroadcastTime.IfNull(DateTime.UtcNow.AddDays(1)) > DateTime.UtcNow.AddHours(-6))
                //    .SelectList(sl =>
                //                sl.Select(s => s.Id).WithAlias(() => resultAlias.Id)
                //                .Select(s=>s.Name).WithAlias(() => resultAlias.Name)
                //                .Select(s => s.DateTime.CreationTime).WithAlias(() => resultAlias.DateTime)
                //                .Select(s=> s.GetType()).WithAlias(() => resultAlias.Type)
                //                .Select(s=>((BroadCastStudyRoom)s).BroadcastTime).WithAlias(() => resultAlias.Scheduled)
                //                .Select(s => s.Identifier).WithAlias(() => resultAlias.ConversationId)
                //                .Select(s=>s.DateTime.UpdateTime).WithAlias(() => resultAlias.LastSession)

                //                .SelectSubQuery(QueryOver.Of<StudyRoomUser>()
                //                    .Where(w=>w.Room.Id == studyRoomAlias.Id)
                //                    .ToRowCountQuery()).WithAlias(() => resultAlias.AmountOfUsers)


                //    )
                //    .OrderBy(()=> studyRoomAlias.DateTime.CreationTime).Desc
                //    .TransformUsing(new SbAliasToBeanResultTransformer<UserStudyRoomDto>())
                //    .UnderlyingCriteria.SetComment(nameof(UserStudyRoomQuery))
                //    .ListAsync<UserStudyRoomDto>(token);


            }
        }
    }
}