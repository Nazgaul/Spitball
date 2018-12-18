using Cloudents.Core.DTOs.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    internal class PendingQuestionsQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<PendingQuestionDto>>
    {

        private readonly IStatelessSession _session;


        public PendingQuestionsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<PendingQuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Question>()
                .Fetch(f => f.User)
                .Where(w => w.User is RegularUser && w.Item.State == ItemState.Pending)
                .Select(s => new PendingQuestionDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Email = s.User.Email,
                    UserId = s.User.Id,
                    ImagesCount = s.Attachments
                }).OrderBy(o => o.Id).ToListAsync(token);
        }
    }
}
