using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class PendingAnswerQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<PendingAnswerDto>>
    {
        private readonly IStatelessSession _session;


        public PendingAnswerQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<PendingAnswerDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Answer>()
                .Fetch(f => f.User)
                .Where(w => w.User is RegularUser && w.Item.State == ItemState.Pending)
                .Select(s => new PendingAnswerDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Email = s.User.Email,
                    UserId = s.User.Id,
                    QuestionText = s.Question.Text,
                    QuestionId = s.Question.Id
                }).OrderBy(o => o.Id).ToListAsync(token);
        }
    }
}
