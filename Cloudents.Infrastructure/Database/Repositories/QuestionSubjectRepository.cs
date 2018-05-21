using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Repositories
{
    [UsedImplicitly]
    public class QuestionSubjectRepository : NHibernateRepository<QuestionSubject>, IQuestionSubjectRepository
    {
        public QuestionSubjectRepository(UnitOfWork.Factory unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<QuestionSubjectDto>> GetAllSubjectAsync(CancellationToken token)
        {

            return await Session.Query<QuestionSubject>()
                .Select(s=> new QuestionSubjectDto
                {
                    Id = s.Id,
                    Subject = s.Subject
                })
                .ToListAsync(token).ConfigureAwait(false);
        }
    }
}
