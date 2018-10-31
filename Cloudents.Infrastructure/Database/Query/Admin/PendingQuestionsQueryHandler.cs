using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    internal class PendingQuestionsQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<PendingQuestionDto>>
    {
   
        private readonly ISession _session;
      

        public PendingQuestionsQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<PendingQuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            PendingQuestionDto dtoAlias = null;
            User userAlias = null;

            return await _session.QueryOver<Question>()
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.State == QuestionState.Pending)
              
                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.Id)
                            .Select(p => p.User.Id).WithAlias(() => dtoAlias.UserId)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.Text)
                            .Select(_ => userAlias.Email).WithAlias(() => dtoAlias.Email)
                )
                .TransformUsing(Transformers.AliasToBean<PendingQuestionDto>())
                .OrderBy(o => o.Id).Asc
                .ListAsync<PendingQuestionDto>(token);

         
        }
    }
}
