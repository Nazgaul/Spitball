using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<QuestionSubject>> GetAllSubjectAsync(CancellationToken token)
        {
            return await Session.Query<QuestionSubject>().ToListAsync(cancellationToken: token).ConfigureAwait(false);
        }
    }
}
