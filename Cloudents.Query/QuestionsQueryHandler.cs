using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using Cloudents.Query.Stuff;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Cloudents.Query
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

            Question questionAlias = null;
            User userAlias = null;
            QuestionFeedDto dto = null;
            return await _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.Id.IsIn(ids))
                .And(w => w.State == ItemState.Ok)
                .SelectList(l => l
                    .Select(s => s.Id).WithAlias(() => dto.Id)
                    .Select(s => s.Subject).WithAlias(() => dto.Subject)
                    .Select(s => s.Price).WithAlias(() => dto.Price)
                    .Select(s => s.Text).WithAlias(() => dto.Text)
                    .Select(s => s.Attachments).WithAlias(() => dto.Files)
                    .SelectSubQuery(QueryOver.Of<Answer>()
                        .Where(w => w.Question.Id == questionAlias.Id && w.State == ItemState.Ok).ToRowCountQuery()).WithAlias(() => dto.Answers)
                    .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                    .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                    .Select(Projections.Property(() => userAlias.Score).As("User.Score"))
                    .Select(Projections.Property(() => userAlias.Image).As("User.Image"))
                    .Select(s => s.Updated).WithAlias(() => dto.DateTime)
                    .Select(s => s.Color).WithAlias(() => dto.Color)
                    .Select(Projections.Conditional(
                        Restrictions.Where(() => questionAlias.CorrectAnswer != null),
                        Projections.Constant(true), Projections.Constant(false))).WithAlias(() => dto.HasCorrectAnswer)
                    .Select(s => s.Language).WithAlias(() => dto.CultureInfo)
                    .Select(Projections.Property(() => questionAlias.Item.VoteCount).As("Vote.Votes"))
                //language
                //vote count
                )
                .TransformUsing(new DeepTransformer<QuestionFeedDto>())
                .ListAsync<QuestionFeedDto>(token);
            //return await _session.Query<Question>()
            //     .Fetch(f => f.User)
            //     .Where(w => ids.Contains(w.Id) && w.Item.State == ItemState.Ok)
            //     .Select(s => new QuestionFeedDto(s.Id,
            //        s.Subject,
            //        s.Price,
            //        s.Text,
            //        s.Attachments,
            //        s.Answers.Count,
            //        new UserDto(s.User.Id, s.User.Name, s.User.Score),
            //        //{
            //        //    Id = s.User.Id,
            //        //    Name = s.User.Name,
            //        //    Image = s.User.Image,
            //        //    Score = s.User.Score
            //        //}, 
            //        s.Updated,
            //        s.Color, s.CorrectAnswer.Id != null, s.Language, s.Item.VoteCount)
            //     )
            //    .ToListAsync(token);

        }
    }
}