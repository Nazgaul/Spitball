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
    public class AdminUserNotesQuery : IQuery<IEnumerable<UserNoteDto>>
    {
        public AdminUserNotesQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class AdminUserNotesQueryHandler : IQueryHandler<AdminUserNotesQuery, IEnumerable<UserNoteDto>>
        {
            private readonly IStatelessSession _session;

            public AdminUserNotesQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<UserNoteDto>> GetAsync(AdminUserNotesQuery query, CancellationToken token)
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
