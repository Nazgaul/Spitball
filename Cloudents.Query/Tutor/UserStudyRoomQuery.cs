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
        public UserStudyRoomQuery(long userId, StudyRoomType type)
        {
            UserId = userId;
            Type = type;
        }

        private long UserId { get; }
        public StudyRoomType Type { get; }


        internal sealed class UserStudyRoomQueryHandler : IQueryHandler<UserStudyRoomQuery, IEnumerable<UserStudyRoomDto>>
        {
            private readonly IStatelessSession _session;

            public UserStudyRoomQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<UserStudyRoomDto>> GetAsync(UserStudyRoomQuery query, CancellationToken token)
            {
                if (query.Type == StudyRoomType.Private)
                {
                    return await _session.Query<PrivateStudyRoom>()
                        .Where(w => w.Tutor.Id == query.UserId || w.Users.Any(a => a.User.Id == query.UserId))
                        .OrderByDescending(o => o.DateTime.CreationTime)
                        .Select(s => new UserStudyRoomDto(
                            s.Name,// s.Name,
                            s.Id,
                            s.DateTime.CreationTime,
                            s.Identifier,
                            s.DateTime.UpdateTime,
                            StudyRoomType.Private,
                            null,
                            s.Users.Select(s2 => s2.User.FirstName).ToList(),
                            s.Price,
                            s.Tutor.Id,
                            s.Tutor.User.Name))
                        .ToListAsync(token);
                }

                return await _session.Query<BroadCastStudyRoom>()
                    .Where(w => w.Tutor.Id == query.UserId || w.Users.Any(a => a.User.Id == query.UserId))
                    .Where(w => w.BroadcastTime > DateTime.UtcNow.AddHours(-1))
                    .Where(w => w.Course.State == ItemState.Ok)
                    .OrderByDescending(o => o.DateTime.CreationTime)
                    .Select(s => new UserStudyRoomDto(
                        s.Course.Name,// s.Name,
                        s.Id,
                        s.DateTime.CreationTime,
                        s.Identifier,
                        s.DateTime.UpdateTime,
                        StudyRoomType.Broadcast,
                        s.BroadcastTime,
                        s.Users.Select(s2 => s2.User.FirstName).ToList(),
                        s.Price,
                        s.Tutor.Id,
                        s.Tutor.User.Name)
                    )
                    .ToListAsync(token);
            }
        }
    }
}