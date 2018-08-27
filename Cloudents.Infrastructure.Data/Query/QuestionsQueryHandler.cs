﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Data.Repositories;
using NHibernate;
using NHibernate.Criterion;

namespace Cloudents.Infrastructure.Data.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionsQueryHandler : IQueryHandler<QuestionsQuery, ResultWithFacetDto<QuestionDto>>
    {
        private readonly ISession _session;

        public QuestionsQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
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
                    .Select(s => s.Updated).WithAlias(() => dto.DateTime)
                    .Select(s => s.Color).WithAlias(() => dto.Color)
                    .Select(Projections.Conditional(
                        Restrictions.Where(()=> questionAlias.CorrectAnswer != null),
                                Projections.Constant(true),Projections.Constant(false) )).WithAlias(() => dto.HasCorrectAnswer)
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

            queryOverObj.OrderBy(o => o.Updated).Desc
                .Skip(query.Page * 50)
                .Take(50);

            var futureQueryOver = queryOverObj.Future<QuestionDto>();

            var facetsFuture = QuestionSubjectRepository.GetSubjects(_session.QueryOver<QuestionSubject>()).Select(s => s.Text)
                .Cacheable().CacheMode(CacheMode.Normal)
                .Future<string>();

            var retVal = await futureQueryOver.GetEnumerableAsync(token).ConfigureAwait(false);
            var facet = facetsFuture.GetEnumerable();

            return new ResultWithFacetDto<QuestionDto>
            {
                Result = retVal,
                Facet = facet
            };
        }
    }
}