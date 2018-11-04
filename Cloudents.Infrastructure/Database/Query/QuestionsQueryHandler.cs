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

namespace Cloudents.Infrastructure.Database.Query
{

    public class QuestionsQueryHandler : IQueryHandler<IdsQuery<long>, IList<QuestionFeedDto>>
    {
        private readonly IStatelessSession _session;

        public QuestionsQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public async Task<IList<QuestionFeedDto>> GetAsync(IdsQuery<long> query, CancellationToken token)
        {
            var ids = query.QuestionIds.ToList();
            return await _session.Query<Question>()
                 .Fetch(f => f.User)
                 .Where(w => ids.Contains(w.Id))
                 .Select(s => new QuestionFeedDto
                 {
                     Id = s.Id,
                     User = new UserDto()
                     {
                         Id = s.User.Id,
                         Name = s.User.Name,
                         Image = s.User.Image
                     },
                     Answers = s.Answers.Count,
                     DateTime = s.Updated,
                     Subject = s.Subject,
                     Text = s.Text,
                     Color = s.Color,
                     Files = s.Attachments,
                     Price = s.Price,
                     HasCorrectAnswer = s.CorrectAnswer.Id != null
                 })
                .ToListAsync(token);

        }
    }
}