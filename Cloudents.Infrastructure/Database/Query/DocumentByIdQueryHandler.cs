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
    public class DocumentByIdQueryHandler : IQueryHandler<DocumentById, DocumentDto>
    {
        private readonly IStatelessSession _session;

        public DocumentByIdQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }
        public Task<DocumentDto> GetAsync(DocumentById query, CancellationToken token)
        {
            return _session.Query<Document>()
                .Where(w => w.Id == query.Id)
                .Select(s => new DocumentDto
                {
                    Name = s.Name,
                    Date = s.TimeStamp.CreationTime,
                    Blob = s.BlobName

                }).SingleOrDefaultAsync(token);
        }
    }
}