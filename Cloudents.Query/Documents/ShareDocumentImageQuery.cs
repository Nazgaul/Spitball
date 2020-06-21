using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Documents
{
    public class ShareDocumentImageQuery : IQuery<ShareDocumentImageDto?>
    {
        public ShareDocumentImageQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }



        internal sealed class ShareDocumentImageQueryHandler : IQueryHandler<ShareDocumentImageQuery, ShareDocumentImageDto?>
        {
            private readonly IStatelessSession _statelessSession;

            public ShareDocumentImageQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            [Cache(TimeConst.Minute * 10, "share-document", false)]
            public Task<ShareDocumentImageDto?> GetAsync(ShareDocumentImageQuery query, CancellationToken token)
            {
                return _statelessSession.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(ShareDocumentImageQuery)))
                    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => new ShareDocumentImageDto()
                    {
                        Name = s.Name,
                        CourseName = s.Course.Id,
                        Type = s.DocumentType

                    }).SingleOrDefaultAsync(token);
            }
        }
    }
}