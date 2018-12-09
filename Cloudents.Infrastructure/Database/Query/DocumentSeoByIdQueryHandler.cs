﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    public class DocumentSeoByIdQueryHandler : IQueryHandler<DocumentById, DocumentSeoDto>
    {
        private readonly IStatelessSession _session;

        public DocumentSeoByIdQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public Task<DocumentSeoDto> GetAsync(DocumentById query, CancellationToken token)
        {
            return _session.Query<Document>()
                .Fetch(f => f.University)
                 .Where(w => w.Id == query.Id && w.State == ItemState.Ok)

                 .Select(s => new DocumentSeoDto(s.Name, s.Course.Name, s.University.Country, s.University.Name, s.Id)
                ).SingleOrDefaultAsync(token);
        }
    }
}