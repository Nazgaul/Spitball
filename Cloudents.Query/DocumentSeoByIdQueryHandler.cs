using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    
    public class DocumentSeoByIdQueryHandler : IQueryHandler<DocumentSeoById, DocumentSeoDto>
    {
        private readonly IStatelessSession _session;

        public DocumentSeoByIdQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public Task<DocumentSeoDto> GetAsync(DocumentSeoById query, CancellationToken token)
        {
            return _session.Query<Document>()
                .Fetch(f => f.University)
                 .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)

                 .Select(s => new DocumentSeoDto
                 {
                     Id = s.Id,
                     Name = s.Name,
                     CourseName = s.Course.Id,
                     UniversityName = s.University.Name
                 }
                ).SingleOrDefaultAsync(token);
        }
    }
}