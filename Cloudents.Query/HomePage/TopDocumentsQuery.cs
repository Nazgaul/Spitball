using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.HomePage
{

    public class TopDocumentsQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        private Country Country { get; }
        private int Count { get; }

        public TopDocumentsQuery(string country, int count)
        {
            Country = country;
            Count = count;
        }

        internal sealed class TopDocumentsHandler : IQueryHandler<TopDocumentsQuery, IEnumerable<DocumentFeedDto>>
        {
            private readonly IStatelessSession _session;
            public TopDocumentsHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            [Cache(TimeConst.Day, "homePage-documents", false)]
            public async Task<IEnumerable<DocumentFeedDto>> GetAsync(TopDocumentsQuery query, CancellationToken token)
            {
                IQueryable<Document> sessionQuery = _session.Query<Document>().Fetch(f => f.University);


                sessionQuery = sessionQuery.Where(w => w.University.Country == query.Country.ToString());

                return await sessionQuery.Where(w => w.IsShownHomePage && w.Status.State == ItemState.Ok).Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    DocumentType = s.DocumentType ?? DocumentType.Document,
                    Duration = s.Duration,
                    University = s.University.Name,
                    Course = s.Course.Id,
                    Snippet = s.Description ?? s.MetaContent,
                    Title = s.Name,
                    User = new DocumentUserDto()
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.ImageName
                    },
                    Views = s.Views,
                    Downloads = s.Downloads,
                    DateTime = s.TimeStamp.UpdateTime,
                    Vote = new VoteDto()
                    {
                        Votes = s.VoteCount
                    },
                    Price = s.Price,
                    Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.Id && x.Action == TransactionActionType.SoldDocument)
                }).Take(query.Count).ToListAsync(token);
            }
        }
    }
}
