using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
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
    public class PendingQuestionsQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<QuestionDto>>
    {
        private readonly ISession _session;
        private readonly IUrlBuilder _urlBuilder;

        public PendingQuestionsQueryHandler(ReadonlySession session, IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
            _session = session.Session;
        }


        public async Task<IEnumerable<QuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            QuestionDto dtoAlias = null;

            Question questionAlias = null;
           

            var questionFuture = _session.QueryOver(() => questionAlias)
                .Where(w => w.State == (QuestionState)Enum.Parse(typeof(QuestionState), "Pending"))

                .SelectList(
                    l =>
                        l.Select(p => p.Id).WithAlias(() => dtoAlias.Id)
                            .Select(p => p.Text).WithAlias(() => dtoAlias.Text)
                            
                )
                .TransformUsing(Transformers.AliasToBean<QuestionDto>())
                .OrderBy(o => o.Id).Asc
                .Future<QuestionDto>();



            return await questionFuture.GetEnumerableAsync(token);
          
        }
    }
}
