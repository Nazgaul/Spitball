﻿using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class NextQuestionsQueryHandler : IQueryHandler<NextQuestionQuery, IEnumerable<QuestionFeedDto>>
    {
        private readonly IStatelessSession _session;

        public NextQuestionsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(NextQuestionQuery query, CancellationToken token)
        {
            QuestionFeedDto dto = null;
            Question questionAlias = null;
            User userAlias = null;


            var detachedQuery = QueryOver.Of<Question>()
                .Select(s => s.Subject)
                .Where(w => w.Id == query.QuestionId && w.Item.State == ItemState.Ok)
                .Take(1);

            return await _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .SelectList(l => l
                    .Select(s => s.Subject).WithAlias(() => dto.Subject)
                    .Select(s => s.Id).WithAlias(() => dto.Id)
                    .Select(s => s.Text).WithAlias(() => dto.Text)
                    .Select(s => s.Price).WithAlias(() => dto.Price)
                    .Select(s => s.Attachments).WithAlias(() => dto.Files)
                    .Select(s => s.Updated).WithAlias(() => dto.DateTime)
                    .Select(s => s.Color).WithAlias(() => dto.Color)

                    .Select(Projections.Property(() => questionAlias.Item.VoteCount).As("Vote.Votes"))
                    .Select(s => s.AnswerCount).WithAlias(() => dto.Answers)
                    .Select(Projections.Conditional(
                        Restrictions.Where(() => questionAlias.CorrectAnswer != null),
                        Projections.Constant(true), Projections.Constant(false))).WithAlias(() => dto.HasCorrectAnswer)
                    .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                    .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                    .Select(Projections.Property(() => userAlias.Image).As("User.Image"))

                )
                .Where(w => w.CorrectAnswer == null)
                .Where(w => w.User.Id != query.UserId)
                .Where(w => w.Id != query.QuestionId)
                .Where(w => w.Item.State == ItemState.Ok)
                .WithSubquery.WhereProperty(x => x.Id)
                .NotIn(QueryOver.Of<Answer>().
                    Where(w => w.User.Id == query.UserId && w.Item.State == ItemState.Ok).Select(s => s.Question.Id))
                .TransformUsing(new DeepTransformer<QuestionFeedDto>())
                .OrderBy(Projections.Conditional(
                    Subqueries.PropertyEq(nameof(Question.Subject), detachedQuery.DetachedCriteria)
                    , Projections.Constant(0), Projections.Constant(1)
                    )).Asc
                .ThenBy(Projections.SqlFunction(SbDialect.RandomOrder, NHibernateUtil.Guid)).Asc
                .Take(3).ListAsync<QuestionFeedDto>(token).ConfigureAwait(false);
        }
    }
}