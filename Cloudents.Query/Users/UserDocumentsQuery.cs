using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserDocumentsQuery : IQuery<ListWithCountDto<DocumentFeedDto>>
    {
        public UserDocumentsQuery(long id, int page, int pageSize, DocumentType? documentType, string course)
        {
            Id = id;
            Page = page;
            PageSize = pageSize;
            DocumentType = documentType;
            Course = course;
        }

        public long Id { get; }
        public int Page { get; }
        public int PageSize { get; }

        public DocumentType? DocumentType { get; }
        public string Course { get;}

        internal sealed class UserDocumentsQueryHandler : IQueryHandler<UserDocumentsQuery, ListWithCountDto<DocumentFeedDto>>
        {
            private readonly IStatelessSession _session;

            public UserDocumentsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<ListWithCountDto<DocumentFeedDto>> GetAsync(UserDocumentsQuery query, CancellationToken token)
            {
                var r = _session.Query<Document>()
                    .Fetch(f => f.User)
                    .ThenFetch(f => f.University)
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok);

                var count = _session.Query<Document>().Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok);

                if (query.DocumentType != null)
                {
                    r = r.Where(w => w.DocumentType == query.DocumentType ||
                                    (query.DocumentType == Core.Enum.DocumentType.Document && w.DocumentType == null)
                                );
                    count = count.Where(w => w.DocumentType == query.DocumentType ||
                                    (query.DocumentType == Core.Enum.DocumentType.Document && w.DocumentType == null));
                }
                if (!string.IsNullOrEmpty(query.Course))
                {
                    r = r.Where(w => w.Course.Id == query.Course);
                    count = count.Where(w => w.Course.Id == query.Course);
                }
                r = r.OrderByDescending(o => o.Boost).ThenByDescending(o => o.TimeStamp.UpdateTime);
                var result = r.Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    User = new DocumentUserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.ImageName,
                    },
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Id,
                    Title = s.Name,
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.User.University.Name,
                    Snippet = s.Description ?? s.MetaContent,
                    Price = s.Price,
                    Vote = new VoteDto
                    {
                        Votes = s.VoteCount
                    },
                    DocumentType = s.DocumentType ?? Core.Enum.DocumentType.Document,
                    Duration = s.Duration,
                    Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.Id && x.Action == TransactionActionType.SoldDocument)

                }
                    )
                .Take(query.PageSize).Skip(query.Page * query.PageSize).ToFuture();

                var countFuture = count
                .GroupBy(g => 1)
    .Select(s => s.Count()).ToFutureValue();


                //var countFuture = count.ToFuture();

                var futureResult = await result.GetEnumerableAsync(token);

                return new ListWithCountDto<DocumentFeedDto>()
                {
                    Result = futureResult,
                    Count = countFuture.Value
                };
            }
        }
    }
}
