using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminPendingQuestionsEmptyQuery : IQuery<IEnumerable<PendingQuestionDto>>
    {
        internal sealed class AdminPendingQuestionsEmptyQueryHandler : IQueryHandler<AdminPendingQuestionsEmptyQuery, IEnumerable<PendingQuestionDto>>
        {
            private readonly IStatelessSession _session;


            public AdminPendingQuestionsEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<PendingQuestionDto>> GetAsync(AdminPendingQuestionsEmptyQuery query, CancellationToken token)
            {
                return await _session.Query<Question>()
                    .Fetch(f => f.User)
                    .Where(w => w.User is User && w.Status.State == ItemState.Pending)
                    .Select(s => new PendingQuestionDto
                    {
                        Id = s.Id,
                        Text = s.Text,
                        Email = s.User.Email,
                        UserId = s.User.Id,
                        ImagesCount = s.Attachments
                    }).OrderBy(o => o.Id).ToListAsync(token);
            }
        }
    }
}
