using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Stuff;

namespace Cloudents.Query
{
    public class UserDocumentsQueryHandler : IQueryHandler<UserDataPagingByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public UserDocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserDataPagingByIdQuery query, CancellationToken token)
        {
            return await _session.Query<Document>()
                .Fetch(f => f.University)
                .Fetch(f => f.User)
                .Where(w => w.User.Id == query.Id && w.State == ItemState.Ok)
                .OrderByDescending(o => o.Id)
                .Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    User = new UserDto(s.User.Id, s.User.Name, s.User.Score),
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Name,
                    TypeStr = s.Type,
                    Professor = s.Professor,
                    Title = s.Name,
                    Source = "Cloudents",
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.University.Name,
                    Snippet = s.MetaContent,
                    Price = s.Price,
                    Vote = new VoteDto()
                    {
                        Votes = s.VoteCount
                    }

                }
                )
                .Take(50).Skip(query.Page * 50)
                .ToListAsync(token);
        }
    }


    public class UserPurchasedDocumentsQueryResult
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int UserScore { get; set; }
        public DateTime? DateTime { get; set; }
        public string Course { get; set; }
        public DocumentType? TypeStr { get; set; }
        public string Professor { get; set; }
        public string Title { get; set; }
        public int? Views { get; set; }
        public decimal? Price { get; set; }
        public int? Downloads { get; set; }
        public string University { get; set; }
        public string Snippet { get; set; }
        public int VoteCount { get; set; }
    }
}