using Cloudents.Core.DTOs;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

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

            //User userAlias = null;
            //QuestionFeedDto dto = null;
            //var result = await _session.QueryOver<Question>()
            //    .JoinAlias(x => x.User, () => userAlias)
            //    .Where(w => w.Id.IsIn(ids))
            //    .And(w => w.Item.State == ItemState.Ok)
            //    .SelectList(l => l
            //        .Select(s => s.Id).WithAlias(() => dto.Id)
            //        .Select(s => s.Subject).WithAlias(() => dto.Subject)
            //        .Select(s => s.Price).WithAlias(() => dto.Price)
            //        .Select(s => s.Text).WithAlias(() => dto.Text)
            //        .Select(s => s.Attachments).WithAlias(() => dto.Files)
            //        .Select(s => s.Attachments).WithAlias(() => dto.Files)
            //    )
            //    .TransformUsing(new DeepTransformer<QuestionFeedDto>())
            //    .ListAsync<QuestionFeedDto>(token);
            return await _session.Query<Question>()
                 .Fetch(f => f.User)
                 .Where(w => ids.Contains(w.Id) && w.Item.State == ItemState.Ok)
                 .Select(s => new QuestionFeedDto(s.Id,
                    s.Subject,
                    s.Price,
                    s.Text,
                    s.Attachments,
                    s.AnswerCount,
                    new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image,
                        Score = s.User.Score
                    }, s.Updated,
                    s.Color, s.CorrectAnswer.Id != null, s.Language, s.Item.VoteCount)
                 )
                .ToListAsync(token);

        }
    }
}