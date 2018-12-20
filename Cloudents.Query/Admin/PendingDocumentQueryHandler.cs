using System.Collections.Generic;
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
    public class PendingDocumentQueryHandler : IQueryHandler<AdminEmptyQuery, IList<PendingDocumentDto>>
    {
        private readonly IStatelessSession _session;


        public PendingDocumentQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IList<PendingDocumentDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Document>()
                .Where(w => w.Item.State == ItemState.Pending)
                .OrderBy(w => w.Id)
                .Take(100)
                .Select(s => new PendingDocumentDto
                {
                    Id = s.Id
                }).ToListAsync(token);
        }
    }
}