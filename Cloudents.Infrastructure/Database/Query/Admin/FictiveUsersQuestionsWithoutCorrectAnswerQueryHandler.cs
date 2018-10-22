using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        private readonly ISession _session;
        private readonly IUrlBuilder _urlBuilder;

        public FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler(ReadonlySession session, IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
            _session = session.Session;
        }



        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            QuestionWithoutCorrectAnswerDto dtoAlias = null;
            AnswerOfQuestionWithoutCorrectAnswer dtoAnswerAlias = null;
            Question questionAlias = null;
            User userAlias = null;

            var questionFuture = _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.CorrectAnswer == null)
                .And(Restrictions.Or(
                    Restrictions.Where(() => userAlias.Fictive),
                    Restrictions.Where(() => questionAlias.Created < DateTime.UtcNow.AddDays(-5))
                ))
                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.Id)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.Text)
                            .Select(_ => userAlias.Fictive).WithAlias(() => dtoAlias.IsFictive)
                )
                .TransformUsing(Transformers.AliasToBean<QuestionWithoutCorrectAnswerDto>())
                .OrderBy(o => o.Id).Asc
                .Future<QuestionWithoutCorrectAnswerDto>();

            var answerFuture = _session.QueryOver<Answer>()
                .JoinAlias(x => x.User, () => userAlias)
                .JoinAlias(x => x.Question, () => questionAlias)
                .Where(() => questionAlias.CorrectAnswer == null)
                .And(Restrictions.Or(
                    Restrictions.Where(() => userAlias.Fictive),
                    Restrictions.Where(() => questionAlias.Created < DateTime.UtcNow.AddDays(-5))))
                .SelectList(
                            l =>
                                l.Select(s => s.Id).WithAlias(() => dtoAnswerAlias.Id)
                                    .Select(s => s.Text).WithAlias(() => dtoAnswerAlias.Text)
                            .Select(s => s.Question.Id).WithAlias(() => dtoAnswerAlias.QuestionId))
                .TransformUsing(Transformers.AliasToBean<AnswerOfQuestionWithoutCorrectAnswer>())
                .OrderBy(x => x.Id).Asc
                .Future<AnswerOfQuestionWithoutCorrectAnswer>();


            var t = await questionFuture.GetEnumerableAsync(token);
            var answers = answerFuture.GetEnumerable().ToLookup(l => l.QuestionId);
            return t.Select(s =>
            {
                s.Url = _urlBuilder.BuildQuestionEndPoint(s.Id);
                s.Answers = answers[s.Id];
                return s;
            });
        }
    }
}