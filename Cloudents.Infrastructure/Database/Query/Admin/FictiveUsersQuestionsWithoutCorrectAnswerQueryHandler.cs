﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    public class FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        private readonly IStatelessSession _session;
        private readonly IUrlBuilder _urlBuilder;

        public FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler(ReadonlyStatelessSession session, IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
            _session = session.Session;
        }



        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            QuestionWithoutCorrectAnswerDto dtoAlias = null;
            Question questionAlias = null;
            Answer answerAlias = null;
            User userAlias = null;


            var t = await _session.QueryOver<Question>(()=> questionAlias)
                .JoinAlias(x => x.Answers, () => answerAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.CorrectAnswer == null)
                //.And(() => userAlias.Fictive)
                .And(Restrictions.Or(
                    Restrictions.Where(() => userAlias.Fictive),
                    //Restrictions.Where(() => answerAlias.Created < DateTime.Now.AddDays(-5))
                    Restrictions.Where(() => questionAlias.Created < DateTime.UtcNow.AddDays(-5))
                    ))
                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.QuestionId)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.QuestionText)
                            .Select(_ => answerAlias.Id).WithAlias(() => dtoAlias.AnswerId)
                            .Select(_ => answerAlias.Text).WithAlias(() => dtoAlias.AnswerText)
                            .Select(p => p.Id).WithAlias(() => dtoAlias.QuestionId)
                            .Select(_ => userAlias.Fictive).WithAlias(() => dtoAlias.IsFictive)
                )
                .TransformUsing(Transformers.AliasToBean<QuestionWithoutCorrectAnswerDto>())
                .OrderBy(o => o.Id).Asc
                .ThenBy(() => answerAlias.Id).Asc
                .ListAsync<QuestionWithoutCorrectAnswerDto>(token).ConfigureAwait(false);

            return t.Select(s =>
            {
                s.Url = _urlBuilder.BuildQuestionEndPoint(s.QuestionId);
                return s;
            });
        }
    }
}