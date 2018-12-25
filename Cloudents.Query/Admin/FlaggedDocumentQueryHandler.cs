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
                .Where(w => w.State == ItemState.Flagged)
                .OrderBy(w => w.Id)
                .Take(100)
                .Select(s => new FlaggedDocumentDto
                {
                    Id = s.Id,
                    Reason = s.FlagReason,
                    FlaggedUserEmail= s.FlaggedUser.Email
                }).ToListAsync(token);
        }
    }
}
