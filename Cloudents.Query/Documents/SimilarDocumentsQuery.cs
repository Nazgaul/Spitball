using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Documents
{
    public class SimilarDocumentsQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        public SimilarDocumentsQuery(long documentId/*, string course*/)
        {
            DocumentId = documentId;
            //Course = course;
        }
        public long DocumentId { get;  }
        //public string Course { get;  }
    }

    internal sealed class SimilarDocumentsQueryHandler : IQueryHandler<SimilarDocumentsQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public SimilarDocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(SimilarDocumentsQuery query, CancellationToken token)
        {
            var t = await _session.Query<Document>()
                .Fetch(f => f.User)
                .Where(w => w.Course.Id ==
                            _session.Query<Document>().Where(w2=>w2.Id == query.DocumentId).Select(s=>s.Course.Id).Single())
                .Where(w => w.University.Id ==
                            _session.Query<Document>().Where(w2 => w2.Id == query.DocumentId).Select(s => s.University.Id).Single())
                .Where(w=> w.Id != query.DocumentId 
                            && w.Status.State == ItemState.Ok)
                .Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    User = new DocumentUserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image,
                    },
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Id,
                    Title = s.Name,
                    Snippet = s.Description ?? s.MetaContent,
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.University.Name,
                    Price = s.Price,
                    Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.Id && x.Action == TransactionActionType.SoldDocument),
                    DocumentType = s.DocumentType ?? DocumentType.Document,
                    Duration = s.Duration,
                    Vote = new VoteDto()
                    {
                        Votes = s.VoteCount
                    }
                }).Take(10).ToListAsync(token);

            return t;
        }
    }
}
