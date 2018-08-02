using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Data.Query.Admin
{
    public class FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler : IQueryHandler<FictiveUsersQuestionsWithoutCorrectAnswerQuery, IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        private readonly IStatelessSession _session;

        public FictiveUsersQuestionsWithoutCorrectAnswerQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }



        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> GetAsync(FictiveUsersQuestionsWithoutCorrectAnswerQuery query, CancellationToken token)
        {
            QuestionWithoutCorrectAnswerDto dtoAlias = null;
            Question questionAlias = null;
            Answer answerAlias = null;
            User userAlias = null;


            return await _session.QueryOver<Question>()
                .JoinAlias(x => x.Answers, () => answerAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.CorrectAnswer == null)
                .And(() => userAlias.Fictive)
                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.QuestionId)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.QuestionText)
                            .Select(_ => answerAlias.Id).WithAlias(() => dtoAlias.AnswerId)
                            .Select(_ => answerAlias.Text).WithAlias(() => dtoAlias.AnswerText)
                            .Select(p => p.Id).WithAlias(() => dtoAlias.QuestionId)
                )
                .TransformUsing(Transformers.AliasToBean<QuestionWithoutCorrectAnswerDto>())
                .OrderBy(o => o.Id).Asc
                .ListAsync<QuestionWithoutCorrectAnswerDto>(token).ConfigureAwait(false);
        }
    }
}