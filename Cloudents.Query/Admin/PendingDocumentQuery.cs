﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class PendingDocumentQuery : IQueryAdmin<IList<PendingDocumentDto>>
    {
        public PendingDocumentQuery(long? documentId, string? country)
        {
            DocumentId = documentId;
            Country = country;
        }

        private long? DocumentId { get;  }
        public string? Country { get;  }

        internal sealed class PendingDocumentQueryHandler : IQueryHandler<PendingDocumentQuery, IList<PendingDocumentDto>>
        {
            private readonly IStatelessSession _session;


            public PendingDocumentQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IList<PendingDocumentDto>> GetAsync(PendingDocumentQuery query, CancellationToken token)
            {
                var dbQuery = _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(PendingDocumentQuery)))
                    .Where(w => w.Status.State == ItemState.Pending);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    dbQuery = dbQuery.Where(w => w.User.Country == query.Country);
                }

                if (query.DocumentId.HasValue)
                {
                    dbQuery = dbQuery.Where(w => w.Id < query.DocumentId.Value);
                }
                return await dbQuery.OrderByDescending(w => w.Id)
                    .Take(50)
                    .Select(s => new PendingDocumentDto
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToListAsync(token);
            }
        }
    }
}
