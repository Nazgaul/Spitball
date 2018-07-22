using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using System.Linq;
using Cloudents.Infrastructure.Data.Repositories;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Data.Query
{
    public class QuestionSubjectQueryHandler : IQueryHandler<QuestionSubjectQuery, IEnumerable<QuestionSubjectDto>>
    {
        private readonly IStatelessSession _session;



        public QuestionSubjectQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }


        public async Task<IEnumerable<QuestionSubjectDto>> GetAsync(QuestionSubjectQuery query, CancellationToken token)
        {
            QuestionSubjectDto dto = null;
           return await QuestionSubjectRepository.GetSubjects(_session.QueryOver<QuestionSubject>()).SelectList(l =>
                l.Select(s => s.Id).WithAlias(() => dto.Id)
                    .Select(s => s.Text).WithAlias(() => dto.Subject)
            ).TransformUsing(Transformers.AliasToBean<QuestionSubjectDto>())
                .ListAsync<QuestionSubjectDto>(token) ;
            //_session.QueryOver<QuestionSubject>()
            //return await _session.Query<QuestionSubject>()
            //    .OrderBy(o => o.Order).ThenBy(o => o.Text)
            //    .Select(s => new QuestionSubjectDto
            //    {
            //        Id = s.Id,
            //        Subject = s.Text
            //    })
            //    .ToListAsync(token).ConfigureAwait(false);
        }
    }
}