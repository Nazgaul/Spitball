using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class UserDocumentsQuery : IQueryAdmin<IEnumerable<UserDocumentsDto>>
    {
        public UserDocumentsQuery(long userId, int page, string country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }
        private long UserId { get; }
        private int Page { get; }
        public string Country { get; }
        internal sealed class UserDocumentsQueryHandler : IQueryHandler<UserDocumentsQuery, IEnumerable<UserDocumentsDto>>
        {
            private readonly IStatelessSession _session;


            public UserDocumentsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            private const int PageSize = 25;

            public async Task<IEnumerable<UserDocumentsDto>> GetAsync(UserDocumentsQuery query, CancellationToken token)
            {

                return await _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(UserDocumentsQuery)))
                    .Fetch(f=>f.Course2)
                        //.Fetch(f => f.University)
                        .Where(w => w.User.Id == query.UserId)
                        .Where(w => w.User.Country == query.Country || string.IsNullOrEmpty(query.Country))
                        .Select(s => new UserDocumentsDto
                        {
                            Course = s.Course2.SearchDisplay,
                            Id = s.Id,
                            Name = s.Name,
                            //University = s.University.Name,
                            State = s.Status.State,
                            Price = s.Price,
                            Created = s.TimeStamp.CreationTime,

                        }).OrderBy(o => o.Id).Take(PageSize).Skip(PageSize * query.Page).ToListAsync(token);
            }
        }
    }
}
