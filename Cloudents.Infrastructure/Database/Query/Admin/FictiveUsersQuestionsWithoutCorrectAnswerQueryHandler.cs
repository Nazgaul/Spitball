using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
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
            Answer answerAlias = null;
            User userAlias = null;


            //this is better but the mapping to answer not working
            //https://github.com/nhibernate/nhibernate-core/issues/1298
            //var q = await _session.Query<Question>()
            //    .Fetch(f => f.User)
            //    .FetchMany(f => f.Answers)
            //    .Where(w => w.CorrectAnswer == null)
            //    .Where(w => w.User.Fictive.GetValueOrDefault() || w.Created < DateTime.UtcNow.AddDays(-5))

            //    .SelectMany(s => s.Answers, (question, answer) => new { question, answer })
            //    .OrderBy(o => o.question.Id).ThenBy(o => o.answer.Id)
            //    .Select(s => new QuestionWithoutCorrectAnswerDto
            //    {
            //        Id = s.question.Id,
            //        Text = s.question.Text,
            //        IsFictive = s.question.User.Fictive.GetValueOrDefault(),

            //        //Answers = new[] { s.answer }
            //        Answers = s.question.Answers.Select(s2 => new AnswerOfQuestionWithoutCorrectAnswer
            //        {
            //            Id = s2.Id,
            //            Text = s2.Text
            //        }).ToList()
            //    })
            //    .ToListAsync(token);


            var questionFuture = _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.Answers, () => answerAlias)
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
               // s.Answers = answers[s.Id];
                return s;
            });
        }
    }
}