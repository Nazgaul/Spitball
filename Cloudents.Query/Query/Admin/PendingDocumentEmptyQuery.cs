﻿using Cloudents.Core.DTOs.Admin;
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
    public class PendingDocumentEmptyQuery : IQuery<IList<PendingDocumentDto>>
    {
        internal sealed class PendingDocumentEmptyQueryHandler : IQueryHandler<PendingDocumentEmptyQuery, IList<PendingDocumentDto>>
        {
            private readonly IStatelessSession _session;


            public PendingDocumentEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<PendingDocumentDto>> GetAsync(PendingDocumentEmptyQuery query, CancellationToken token)
            {
                return await _session.Query<Document>()
                    .Where(w => w.Status.State == ItemState.Pending)
                    .OrderBy(w => w.Id)
                    .Take(100)
                    .Select(s => new PendingDocumentDto
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToListAsync(token);
            }
        }
    }
}
