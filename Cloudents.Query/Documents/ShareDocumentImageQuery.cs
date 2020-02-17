using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Tutor;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Documents
{
    public class ShareDocumentImageQuery : IQuery<ShareDocumentImageDto>
    {
        public ShareDocumentImageQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }


        internal sealed class ShareDocumentImageQueryHandler : IQueryHandler<ShareDocumentImageQuery, ShareDocumentImageDto>
        {
            private readonly IStatelessSession _statelessSession;

            public ShareDocumentImageQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }

            public async Task<ShareDocumentImageDto> GetAsync(ShareDocumentImageQuery query, CancellationToken token)
            {
                return await _statelessSession.Query<Document>()
                    .Where(w => w.Id == query.Id)
                    .Select(s => new ShareDocumentImageDto()
                    {
                        Name = s.Name,
                        CourseName = s.Course.Id,
                        Type = s.DocumentType ?? DocumentType.Document

                    }).SingleOrDefaultAsync(token);
            }
        }
    }
}