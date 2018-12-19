using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;

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
                 .Where(w => w.Id == query.Id && w.Item.State == ItemState.Ok)

                 .Select(s => new DocumentSeoDto
                 {
                     Id = s.Id,
                     Name = s.Name,
                     Country = s.University.Country,
                     MetaContent = s.MetaContent,
                     CourseName = s.Course.Name,
                     UniversityName = s.University.Name
                 }
                // (s.Name, s.Course.Name, s.University.Country, s.University.Name, s.Id)
                ).SingleOrDefaultAsync(token);
        }
    }
}