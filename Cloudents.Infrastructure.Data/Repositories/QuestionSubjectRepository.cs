using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    [UsedImplicitly]
    public class QuestionSubjectRepository : NHibernateRepository<QuestionSubject>, IQuestionSubjectRepository
    {
        public QuestionSubjectRepository(IIndex<Core.Enum.Database, IUnitOfWork> unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<QuestionSubjectDto>> GetAllSubjectAsync(CancellationToken token)
        {
            return await Session.Query<QuestionSubject>()
                .Select(s=> new QuestionSubjectDto
                {
                    Id = s.Id,
                    Subject = s.Text
                })
                .ToListAsync(token).ConfigureAwait(false);
        }
    }
}
