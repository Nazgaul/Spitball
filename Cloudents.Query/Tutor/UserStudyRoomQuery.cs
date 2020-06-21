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
                     .Where(w => w.Tutor.Id == query.UserId || w.Users.Any(a => a.User.Id == query.UserId))
                     .Where(w => (((BroadCastStudyRoom)w).BroadcastTime != null ? ((BroadCastStudyRoom)w).BroadcastTime : DateTime.UtcNow.AddDays(1)) > DateTime.UtcNow.AddHours(-1))
                     .OrderByDescending(o => o.DateTime.CreationTime)
                     .Select(s => new UserStudyRoomDto(
                         s.Name,
                         s.Id,
                         s.DateTime.CreationTime,
                         s.Identifier,
                         s.DateTime.UpdateTime,
                         (s is BroadCastStudyRoom) ? StudyRoomType.Broadcast : StudyRoomType.Private,
                         (s as BroadCastStudyRoom) != null ? ((BroadCastStudyRoom)s).BroadcastTime : new DateTime?(),
                         s.Users.Select(s2 => s2.User.FirstName).ToList(), 
                         s.Price))
                     .ToListAsync(token);
            }
        }
    }
}