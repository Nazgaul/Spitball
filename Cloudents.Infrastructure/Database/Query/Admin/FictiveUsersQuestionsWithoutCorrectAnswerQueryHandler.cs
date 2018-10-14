﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Criterion;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
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


            var t = await _session.QueryOver(()=> questionAlias)
                .JoinAlias(x => x.Answers, () => answerAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.CorrectAnswer == null)
                .And(Restrictions.Or(
                    Restrictions.Where(() => userAlias.Fictive),
                    Restrictions.Where(() => questionAlias.Created < DateTime.UtcNow.AddDays(-5))
                    ))
                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.QuestionId)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.QuestionText)
                            .Select(Projections.Property(() => answerAlias.Id).As($"{nameof(QuestionWithoutCorrectAnswerDto.Answer)}.{nameof(AnswerOfQuestionWithoutCorrectAnswer.Id)}"))
                            .Select(Projections.Property(() => answerAlias.Text).As($"{nameof(QuestionWithoutCorrectAnswerDto.Answer)}.{nameof(AnswerOfQuestionWithoutCorrectAnswer.Text)}"))
                            .Select(p => p.Id).WithAlias(() => dtoAlias.QuestionId)
                            .Select(_ => userAlias.Fictive).WithAlias(() => dtoAlias.IsFictive)
                )
               .TransformUsing(new DeepTransformer<QuestionWithoutCorrectAnswerDto>())
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