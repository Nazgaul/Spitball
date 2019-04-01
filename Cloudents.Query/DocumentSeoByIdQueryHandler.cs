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
    public class DocumentOldIdToNewIdQueryHandler : IQueryHandler<DocumentSeoByOldId, DocumentSeoDto>
    {
        private readonly IStatelessSession _session;

        public DocumentOldIdToNewIdQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public Task<DocumentSeoDto> GetAsync(DocumentSeoByOldId query, CancellationToken token)
        {
            return _session.Query<Document>()
                .Fetch(f => f.University)
                .Where(w => w.OldId == query.OldId && w.Status.State == ItemState.Ok)

                .Select(s => new DocumentSeoDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Country = s.University.Country,
                        MetaContent = s.MetaContent,
                        CourseName = s.Course.Id,
                        UniversityName = s.University.Name
                    }
                    // (s.Name, s.Course.Name, s.University.Country, s.University.Name, s.Id)
                ).SingleOrDefaultAsync(token);
        }
    }
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
                     Country = s.University.Country,
                     MetaContent = s.MetaContent,
                     CourseName = s.Course.Id,
                     UniversityName = s.University.Name
                 }
                // (s.Name, s.Course.Name, s.University.Country, s.University.Name, s.Id)
                ).SingleOrDefaultAsync(token);
        }
    }
}