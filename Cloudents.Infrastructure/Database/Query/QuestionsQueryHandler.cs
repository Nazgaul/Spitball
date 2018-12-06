using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Database.Query
{

    public class QuestionsQueryHandler : IQueryHandler<IdsQuery<long>, IList<QuestionFeedDto>>
    {
        private readonly IStatelessSession _session;

        public QuestionsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IList<QuestionFeedDto>> GetAsync(IdsQuery<long> query, CancellationToken token)
        {
            var ids = query.QuestionIds.ToList();
            
            return await _session.Query<QuestionApproved>()
                 .Fetch(f => f.User)
                 .Where(w => ids.Contains(w.Id))
                 //.Where(w => w.State == null || w.State == ItemState.Ok)
                 .Select(s => new QuestionFeedDto(s.Id,
                    s.Subject,
                    s.Price,
                    s.Text,
                    s.Attachments,
                    s.Answers.Count,
                    new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image
                    }, s.Updated,
                    s.Color, s.CorrectAnswer.Id != null, s.Language)
                 )
                .ToListAsync(token);

        }
    }
}