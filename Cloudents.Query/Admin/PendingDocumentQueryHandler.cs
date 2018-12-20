using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs.Admin;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query.Admin;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query.Admin
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