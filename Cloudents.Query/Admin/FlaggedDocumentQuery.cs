using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class FlaggedDocumentQuery : IQueryAdmin2<IList<FlaggedDocumentDto>>
    {
        public FlaggedDocumentQuery(Country? country)
        {
            Country = country;
        }
        public Country? Country { get; }
        internal sealed class FlaggedDocumentEmptyQueryHandler : IQueryHandler<FlaggedDocumentQuery, IList<FlaggedDocumentDto>>
        {
            private readonly IStatelessSession _session;


            public FlaggedDocumentEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<FlaggedDocumentDto>> GetAsync(FlaggedDocumentQuery query, CancellationToken token)
            {
                var documents = _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(FlaggedDocumentQuery)))
                    .Where(w => w.Status.State == ItemState.Flagged);
                if (query.Country != null)
                {
                    documents = documents.Where(w => w.User.SbCountry == query.Country);
                }

                return await documents.OrderByDescending(w => w.Id)
                .Take(100)
                .Select(s => new FlaggedDocumentDto
                {
                    Id = s.Id,
                    Reason = s.Status.FlagReason!,
                    FlaggedUserEmail = s.Status.FlaggedUser!.Email
                }).ToListAsync(token);
            }
        }
    }
}
