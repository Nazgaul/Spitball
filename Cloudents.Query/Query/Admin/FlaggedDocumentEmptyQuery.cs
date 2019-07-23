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
    public class FlaggedDocumentEmptyQuery : IQuery<IList<FlaggedDocumentDto>>
    {
        internal sealed class FlaggedDocumentEmptyQueryHandler : IQueryHandler<FlaggedDocumentEmptyQuery, IList<FlaggedDocumentDto>>
        {
            private readonly IStatelessSession _session;


            public FlaggedDocumentEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<FlaggedDocumentDto>> GetAsync(FlaggedDocumentEmptyQuery query, CancellationToken token)
            {
                return await _session.Query<Document>()
                    .Where(w => w.Status.State == ItemState.Flagged)
                    .OrderByDescending(w => w.Id)
                    .Take(100)
                    .Select(s => new FlaggedDocumentDto
                    {
                        Id = s.Id,
                        Reason = s.Status.FlagReason,
                        FlaggedUserEmail = s.Status.FlaggedUser.Email
                    }).ToListAsync(token);
            }
        }
    }
}
