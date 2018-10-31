using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    class PendingQuestionsQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<PendingQuestionDto>>
    {
   
        private readonly ISession _session;
      

        public PendingQuestionsQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<PendingQuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            PendingQuestionDto dtoAlias = null;
            Question questionAlias = null;
            User userAlias = null;

            var questionFuture = _session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .Where(w => w.State == (QuestionState)Enum.Parse(typeof(QuestionState), "Pending"))
              
                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.Id)
                            .Select(p => p.User.Id).WithAlias(() => dtoAlias.UserId)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.Text)
                            .Select(_ => userAlias.Email).WithAlias(() => dtoAlias.Email)
                )
                .TransformUsing(Transformers.AliasToBean<PendingQuestionDto>())
                .OrderBy(o => o.Id).Asc
                .Future<PendingQuestionDto>();

          
            var t = await questionFuture.GetEnumerableAsync(token);

            return t;
        }
    }
}
