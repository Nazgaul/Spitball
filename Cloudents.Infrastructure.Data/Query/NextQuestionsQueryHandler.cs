using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Criterion;

namespace Cloudents.Infrastructure.Data.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class NextQuestionsQueryHandler : IQueryHandler<NextQuestionQuery, IEnumerable<QuestionDto>>
    {
        private readonly IStatelessSession _session;

        public NextQuestionsQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<QuestionDto>> GetAsync(NextQuestionQuery query, CancellationToken token)
        {
            QuestionDto dto = null;
            QuestionSubject commentAlias = null;
            Question questionAlias = null;
            User userAlias = null;

            return await _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.Subject, () => commentAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .SelectList(l => l
                    .Select(_ => commentAlias.Text).WithAlias(() => dto.Subject)
                    .Select(s => s.Id).WithAlias(() => dto.Id)
                    .Select(s => s.Text).WithAlias(() => dto.Text)
                    .Select(s => s.Price).WithAlias(() => dto.Price)
                    .Select(s => s.Attachments).WithAlias(() => dto.Files)
                    .Select(s => s.Created).WithAlias(() => dto.DateTime)
                    .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                    .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                    .Select(Projections.Property(() => userAlias.Image).As("User.Image"))
                    .SelectSubQuery(QueryOver.Of<Answer>()
                        .Where(w => w.Question.Id == questionAlias.Id).ToRowCountQuery()).WithAlias(() => dto.Answers)

                )
                .Where(w => w.CorrectAnswer == null)
                .Where(w => w.User.Id != query.UserId)
                .Where(w => w.Id != query.QuestionId)
                .WithSubquery.Where(p => p.Subject.Id == QueryOver.Of<Question>()
                                             .Select(s => s.Subject.Id)
                                             .Where(w => w.Id == query.QuestionId)
                                             .Take(1).As<int>())

                .TransformUsing(new DeepTransformer<QuestionDto>())
                .OrderByRandom()
                .Take(3).ListAsync<QuestionDto>(token).ConfigureAwait(false);
        }
    }
}