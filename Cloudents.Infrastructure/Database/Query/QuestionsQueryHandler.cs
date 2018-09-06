using System.Collections.Generic;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.EventHandler;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Database.Repositories;
using NHibernate;
using NHibernate.Criterion;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionsQueryHandler : IQueryHandler<QuestionsQuery, IEnumerable<QuestionDto>>
    {
        private readonly ISession _session;

        public QuestionsQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }

        [Cache(TimeConst.Minute * 5, RemoveQuestionCacheEventHandler.CacheRegion, false)]
        public async Task<IEnumerable<QuestionDto>> GetAsync(QuestionsQuery query, CancellationToken token)
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
                    .Select(s => s.Updated).WithAlias(() => dto.DateTime)
                    .Select(s => s.Color).WithAlias(() => dto.Color)
                    .Select(Projections.Conditional(
                        Restrictions.Where(() => questionAlias.CorrectAnswer != null),
                                Projections.Constant(true), Projections.Constant(false))).WithAlias(() => dto.HasCorrectAnswer)
                    .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                    .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                    .Select(Projections.Property(() => userAlias.Image).As("User.Image"))
                    .SelectSubQuery(QueryOver.Of<Answer>()
                        .Where(w => w.Question.Id == questionAlias.Id).ToRowCountQuery()).WithAlias(() => dto.Answers)

                )
                //.Where(w => w.CorrectAnswer == null)
                .TransformUsing(new DeepTransformer<QuestionDto>());
            if (query.Source != null)
            {
                queryOverObj.WhereRestrictionOn(() => commentAlias.Text).IsIn(query.Source.ToArray<object>());
            }

            if (!string.IsNullOrEmpty(query.Term))
            {
                queryOverObj.Where(new FullTextCriterion(Projections.Property<Question>(x => x.Text),
                    query.Term));
            }


            switch (query.Filter)
            {
                case QuestionFilter.Unanswered:
                    queryOverObj.WithSubquery.WhereNotExists(QueryOver.Of<Answer>()
                        .Where(tx => tx.Question.Id == questionAlias.Id).Select(x => x.Id));
                    break;
                case QuestionFilter.Answered:
                    queryOverObj.WithSubquery.WhereExists(QueryOver.Of<Answer>()
                            .Where(tx => tx.Question.Id == questionAlias.Id).Select(x => x.Id))
                        .And(t => t.CorrectAnswer == null);
                    break;
                case QuestionFilter.Sold:
                    queryOverObj.Where(w => w.CorrectAnswer != null);
                    break;
            }

            queryOverObj.OrderBy(o => o.Updated).Desc
                .Skip(query.Page * 50)
                .Take(50);

            //var futureQueryOver = queryOverObj.Future<QuestionDto>();

            //var facetsFuture = QuestionSubjectRepository.GetSubjects(_session.QueryOver<QuestionSubject>()).Select(s => s.Text)
            //    .Cacheable().CacheMode(CacheMode.Normal)
            //    .Future<string>();

            return await queryOverObj.ListAsync<QuestionDto>(token);
        }
    }
}