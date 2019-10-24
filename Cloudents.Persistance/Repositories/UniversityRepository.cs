using Cloudents.Core.Entities;
using Cloudents.Persistence.Repositories;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public UniversityRepository(ISession session) : base(session)
        {

        }
        public async Task MigrateUniversityAsync(Guid universityToRemove, Guid universityToKeep, CancellationToken token)
        {
            var uniToKeep = await LoadAsync(universityToKeep, token);
            
            var updateUsers = await Session.Query<User>().Where(w => w.University.Id == universityToRemove).UpdateBuilder()
                .Set(s => s.University, uniToKeep).UpdateAsync(token);

            var updateDocuments = await Session.Query<Document>().Where(w => w.University.Id == universityToRemove).UpdateBuilder()
                .Set(s => s.University, uniToKeep).UpdateAsync(token);

            var updateQuestions = await Session.Query<Question>().Where(w => w.University.Id == universityToRemove).UpdateBuilder()
                .Set(s => s.University, uniToKeep).UpdateAsync(token);

            var uniToRemove = await LoadAsync(universityToRemove, token);

            await DeleteAsync(uniToRemove, token);
        }

        public async Task DeleteAsync(Guid universityId, CancellationToken token)
        {
            var university = await LoadAsync(universityId, token);

            var updateUser = await Session.Query<User>().Where(w => w.University.Id == universityId).UpdateBuilder()
                .Set(s => s.University, value : null).UpdateAsync(token);

            var updateDocuments = await Session.Query<Document>().Where(w => w.University.Id == universityId).UpdateBuilder()
                .Set(s => s.University, value : null).UpdateAsync(token);

            var updateQuestions = await Session.Query<Question>().Where(w => w.University.Id == universityId).UpdateBuilder()
                .Set(s => s.University, value: null).UpdateAsync(token);

            await DeleteAsync(university, token);
        }
    }
}
