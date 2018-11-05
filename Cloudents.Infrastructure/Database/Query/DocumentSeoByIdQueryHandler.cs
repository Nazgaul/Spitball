using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;

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
            return Task.FromResult<DocumentSeoDto>(null);
            //return _session.Query<Document>()
            //    .Fetch(f=>f.Course)
            //    .ThenFetch(f=>f.University)

            //     .Where(w => w.Id == query.Id && !w.IsDeleted)

            //     .Select(s => new DocumentSeoDto
            //     {
            //        Name = s.Name,
            //         Country = s.Course.University != null ? s.Course.University.Country : null,
            //         Discriminator = s.Discriminator,
            //         CourseName = s.Course.Name,
            //         Description = s.Content,
            //         ImageUrl = s.BlobName
            //     }).SingleOrDefaultAsync(token);
        }
    }
}