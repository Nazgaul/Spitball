using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class UserNotesQuery : IQuery<IEnumerable<UserNoteDto>>
    {
        public UserNotesQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class UserNotesQueryHandler : IQueryHandler<UserNotesQuery, IEnumerable<UserNoteDto>>
        {
            private readonly IStatelessSession _session;

            public UserNotesQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<UserNoteDto>> GetAsync(UserNotesQuery query, CancellationToken token)
            {
                return await _session.Query<AdminNote>()
                    .Fetch(f => f.AdminUser)
                    .Where(w => w.User.Id == query.UserId)
                    .OrderByDescending(o => o.TimeStamp.CreationTime)
                    .Select(s => new UserNoteDto()
                    {
                        Text = s.Text,
                        Created = s.TimeStamp.CreationTime,
                        AdminUser = s.AdminUser.Email
                    }).ToListAsync();
            }
        }
    }
}
