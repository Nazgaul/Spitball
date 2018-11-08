using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Query
{
    public class DocumentSeoByIdQueryHandler :IQueryHandler<DocumentById, DocumentSeoDto>
    {
        private readonly IStatelessSession _session;

        public DocumentSeoByIdQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }
        public Task<DocumentSeoDto> GetAsync(DocumentById query, CancellationToken token)
        {
            //V7 - need to fix
            return _session.Query<Document>()
                .Fetch(f=>f.University)
                //.ThenFetch(f => f.University)
                 .Where(w => w.Id == query.Id)

                 .Select(s => new DocumentSeoDto
                 {
                     Name = s.Name,
                     Country = s.University.Country,
                     CourseName = s.Course.Name,
                     //Description = s.Content,
                 }).SingleOrDefaultAsync(token);
        }
    }
}