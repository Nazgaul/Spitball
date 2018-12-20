using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs.Admin;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query.Admin;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    class FlaggedAnswerQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<FlaggedAnswerDto>>
    {

        private readonly IStatelessSession _session;


        public FlaggedAnswerQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<FlaggedAnswerDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Answer>()
                .Fetch(f => f.User)
                .Where(w => w.Item.State == ItemState.Flagged)
                .Select(s => new FlaggedAnswerDto
                {
                    Id = s.Id,
                    Reason = s.Item.FlagReason,
                    FlaggedUserEmail = s.Item.FlaggedUser.Email
                }).OrderBy(o => o.Id).ToListAsync(token);
        }
    }
}
