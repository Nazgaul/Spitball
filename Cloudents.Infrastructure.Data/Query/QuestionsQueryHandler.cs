using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Query
{
    public class QuestionsQueryHandler : IQueryHandlerAsync<QuestionsQuery, ResultWithFacetDto<QuestionDto>>
    {
        private readonly ISession _session;

        public QuestionsQueryHandler(ISession session)
        {
            _session = session;
        }

        public async Task<ResultWithFacetDto<QuestionDto>> GetAsync(QuestionsQuery query, CancellationToken token)
        {
            QuestionDto dto = null;
            QuestionSubject commentAlias = null;
            Question questionAlias = null;
            User userAlias = null;

            var queryOverObj = _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.Subject, () => commentAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .SelectList(l => l
                    .Select(_ => commentAlias.Text).WithAlias(() => dto.Subject)
                    .Select(s => s.Id).WithAlias(() => dto.Id)
                    .Select(s => s.Text).WithAlias(() => dto.Text)
                    .Select(s => s.Price).WithAlias(() => dto.Price)
                    .Select(s => s.Attachments).WithAlias(() => dto.Files)
                    .Select(s => s.Created).WithAlias(() => dto.DateTime)
                    //.SelectCount(s => s.Answers).WithAlias(() => dto.Answers)
                    //.Select(_ => userAlias.Name).As("User.Name")
                    .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                    .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                    .Select(Projections.Property(() => userAlias.Image).As("User.Image"))
                    .SelectSubQuery(QueryOver.Of<Answer>()
                        .Where(w => w.Question.Id == questionAlias.Id).ToRowCountQuery()).WithAlias(() => dto.Answers)

                )
                .Where(w => w.CorrectAnswer == null)
                .TransformUsing(new DeepTransformer<QuestionDto>());
            if (query.Source != null)
            {
                queryOverObj.WhereRestrictionOn(() => commentAlias.Text).IsIn(query.Source);
            }

            if (!string.IsNullOrEmpty(query.Term))
            {
                queryOverObj.Where(new FullTextCriterion(Projections.Property<Question>(x => x.Text),
                    query.Term));
            }

            queryOverObj.OrderBy(o => o.Id).Desc
                .Skip(query.Page * 50)
                .Take(50);

            var futureQueryOver = queryOverObj.Future<QuestionDto>();

            var facetsFuture = _session.Query<QuestionSubject>().Select(s => s.Text).ToFuture();
            var retVal = await futureQueryOver.GetEnumerableAsync(token).ConfigureAwait(false);
            var facet = await facetsFuture.GetEnumerableAsync(token).ConfigureAwait(false);

            return new ResultWithFacetDto<QuestionDto>
            {
                Result = retVal,
                Facet = facet
            };
            // return _questionRepository.GetQuestionsAsync(query, token);
        }
    }
}