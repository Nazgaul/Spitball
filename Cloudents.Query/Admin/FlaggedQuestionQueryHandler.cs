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
    public class FlaggedQuestionQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<FlaggedQuestionDto>>
    {

        private readonly IStatelessSession _session;


        public FlaggedQuestionQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<FlaggedQuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Question>()
                .Fetch(f => f.User)
                .Where(w => w.User is User && w.Status.State == ItemState.Flagged)
                .Select(s => new FlaggedQuestionDto
                {
                    Id = s.Id,
                    Reason = s.Status.FlagReason,
                    Text = s.Text,
                    FlaggedUserEmail = s.Status.FlaggedUser.Email
                }).OrderBy(o => o.Id).ToListAsync(token);
        }
    }
}
