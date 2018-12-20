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
    internal class FlaggedDocumentQueryHandler : IQueryHandler<AdminEmptyQuery, IList<FlaggedDocumentDto>>
    {
        private readonly IStatelessSession _session;


        public FlaggedDocumentQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IList<FlaggedDocumentDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Document>()
                .Where(w => w.Item.State == ItemState.Flagged)
                .OrderBy(w => w.Id)
                .Take(100)
                .Select(s => new FlaggedDocumentDto
                {
                    Id = s.Id,
                    Reason = s.Item.FlagReason,
                    FlaggedUserEmail= s.Item.FlaggedUser.Email
                }).ToListAsync(token);
        }
    }
}
