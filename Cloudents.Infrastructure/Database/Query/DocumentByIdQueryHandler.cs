using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Query
{
    [UsedImplicitly]
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
                .Fetch(f=>f.University)
                .Fetch(f=>f.User)
                .Where(w => w.Id == query.Id)
                .Select(s => new DocumentDto
                {
                    Name = s.Name,
                    Date = s.TimeStamp.CreationTime,
                    Blob = s.BlobName,
                    University = s.University.Name,
                    Type = s.Type,
                    Pages = s.PageCount.GetValueOrDefault(),
                    Professor = s.Professor,
                    Views = s.Views,
                    Owner = s.User.Name,
                    Course = s.Course.Name
                })
                .SingleOrDefaultAsync(token);
        }
    }
}