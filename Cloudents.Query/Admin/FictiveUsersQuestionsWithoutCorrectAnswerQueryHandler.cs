using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Cloudents.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler : IQueryHandler<AdminPageQuery, IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        private const int PageSize = 30;
        private readonly IStatelessSession _session;
        private readonly IUrlBuilder _urlBuilder;

        public FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler(QuerySession session, IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> GetAsync(AdminPageQuery query, CancellationToken token)
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
                    .And(x=>x.Status.State == ItemState.Ok)
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
                .Where(w=>w.Status.State == ItemState.Ok)
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
                s.Url = _urlBuilder.BuildQuestionEndPoint(s.Id);
                s.Answers = answers[s.Id];
                return s;
            }).Where(s => s.Answers.Any());
        }
    }
}