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
    class PendingAnswerQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<PendingAnswerDto>>
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
                    UserId = s.User.Id
                }).OrderBy(o => o.Id).ToListAsync(token);
        }
    }
}
