using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<IEnumerable<QuestionSubject>> GetAllSubjectAsync()
        {
            return await _session.Query<QuestionSubject>().ToListAsync().ConfigureAwait(false);
        }
    }
}
