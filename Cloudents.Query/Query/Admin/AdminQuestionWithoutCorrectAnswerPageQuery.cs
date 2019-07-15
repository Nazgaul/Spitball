using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Cloudents.Query.Query.Admin
{
    public class AdminQuestionWithoutCorrectAnswerPageQuery : IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        public AdminQuestionWithoutCorrectAnswerPageQuery(int page)
        {
            Page = page;

        }

        private int Page { get; }

        internal sealed class FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler : IQueryHandler<AdminQuestionWithoutCorrectAnswerPageQuery, IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        private const int PageSize = 30;
        private readonly IStatelessSession _session;


        public FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> GetAsync(AdminQuestionWithoutCorrectAnswerPageQuery query, CancellationToken token)
        {

            QuestionWithoutCorrectAnswerDto dtoAlias = null;
            AnswerOfQuestionWithoutCorrectAnswer dtoAnswerAlias = null;
            Question questionAlias = null;
            BaseUser userAlias = null;

            var questions = await _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.CorrectAnswer == null)
                .Where(w => w.Status.State == ItemState.Ok)
                .WithSubquery.WhereExists(QueryOver.Of<Answer>().Where(w => w.Question.Id == questionAlias.Id)
                    .And(x => x.Status.State == ItemState.Ok)
                    .Select(s => s.Id))
                .And(Restrictions.Or(
                    Restrictions.Where(() => userAlias.Fictive),
                    Restrictions.Where(() => questionAlias.Created < DateTime.UtcNow.AddDays(-5))
                ))
                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.Id)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.Text)
                            .Select(p => p.Updated).WithAlias(() => dtoAlias.Create)
                            .Select(p => p.Attachments).WithAlias(() => dtoAlias.ImagesCount)
                            .Select(_ => userAlias.Fictive).WithAlias(() => dtoAlias.IsFictive)
                )
                .TransformUsing(Transformers.AliasToBean<QuestionWithoutCorrectAnswerDto>())
                .OrderBy(o => o.Id).Asc
                .Take(PageSize).Skip(PageSize * query.Page)
                .ListAsync<QuestionWithoutCorrectAnswerDto>(token);


            var answersResult = await _session.QueryOver<Answer>()
                .Where(w => w.Question.Id.IsIn(questions.Select(s => s.Id).ToArray()))
                .Where(w => w.Status.State == ItemState.Ok)
                .SelectList(
                            l =>
                                l.Select(s => s.Id).WithAlias(() => dtoAnswerAlias.Id)
                                    .Select(s => s.Text).WithAlias(() => dtoAnswerAlias.Text)
                            .Select(s => s.Question.Id).WithAlias(() => dtoAnswerAlias.QuestionId)
                            .Select(p => p.Attachments).WithAlias(() => dtoAnswerAlias.ImagesCount))
                .TransformUsing(Transformers.AliasToBean<AnswerOfQuestionWithoutCorrectAnswer>())
                .OrderBy(x => x.Id).Asc
                .ListAsync<AnswerOfQuestionWithoutCorrectAnswer>(token);

            var answers = answersResult.ToLookup(l => l.QuestionId);
            return questions.Select(s =>
            {
                s.Answers = answers[s.Id];
                return s;
            }).Where(s => s.Answers.Any());
        }
    }
}
}
