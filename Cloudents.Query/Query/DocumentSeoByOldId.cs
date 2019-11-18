using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class DocumentSeoByOldId : IQuery<DocumentSeoDto>
    {
        public DocumentSeoByOldId(long oldId)
        {
            OldId = oldId;
        }

        private long OldId { get; }


        internal sealed class DocumentOldIdToNewIdQueryHandler : IQueryHandler<DocumentSeoByOldId, DocumentSeoDto>
        {
            private readonly IStatelessSession _session;

            public DocumentOldIdToNewIdQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public Task<DocumentSeoDto> GetAsync(DocumentSeoByOldId query, CancellationToken token)
            {
                return _session.Query<Document>()
                    //.Fetch(f => f.University)
                    .Where(w => w.OldId == query.OldId && w.Status.State == ItemState.Ok)

                    .Select(s => new DocumentSeoDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        //Country = s.University.Country,
                        //MetaContent = s.MetaContent,
                        CourseName = s.Course.Id,
                        //UniversityName = s.University.Name
                    }
                    // (s.Name, s.Course.Name, s.University.Country, s.University.Name, s.Id)
                    ).SingleOrDefaultAsync(token);
            }
        }
    }
}