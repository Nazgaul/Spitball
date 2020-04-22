using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class CourseRepository : NHibernateRepository<Course>, ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {
        }

        public async Task<IEnumerable<Course>> GetCoursesBySubjectIdAsync(long subjectId, CancellationToken token)
        {
            return await Session.Query<Course>()
                .Fetch(f => f.Subject)
                .Where(w => w.Subject.Id == subjectId)
                .ToListAsync(token);
        }

        public async Task MigrateCourseAsync(string courseToKeepId, string courseToRemoveId, CancellationToken token)
        {
            var courseToKeep = await LoadAsync(courseToKeepId, token);

            await Session.Query<Document>().Where(w => w.Course.Id == courseToRemoveId).UpdateBuilder()
               .Set(s => s.Course, courseToKeep).UpdateAsync(token);

            await Session.Query<Question>().Where(w => w.Course.Id == courseToRemoveId).UpdateBuilder()
               .Set(s => s.Course, courseToKeep).UpdateAsync(token);


            var subQuery = Session.Query<UserCourse>().Where(w => w.Course.Id == courseToKeepId)
                .Select(s => s.User.Id);

            await Session.Query<UserCourse>()
                .Where(w => w.Course.Id == courseToRemoveId)
                .Where(w => !subQuery.Contains(w.User.Id))
                .UpdateBuilder()
                .Set(s => s.Course, courseToKeep)
                .UpdateAsync(token);

            await Session.Query<UserCourse>().Where(w => w.Course.Id == courseToRemoveId).DeleteAsync(token);
            var courseToRemove = await LoadAsync(courseToRemoveId, token);
            await DeleteAsync(courseToRemove, token);
        }

        public async Task RenameCourseAsync(string courseName, string newName, CancellationToken token)
        {
            var newCourse = new Course(newName);
            await AddAsync(newCourse, token);
            await Session.FlushAsync(token);

            await Session.Query<UserCourse>().Where(w => w.Course.Id == courseName).UpdateBuilder()
               .Set(s => s.Course, newCourse).UpdateAsync(token);


            await Session.Query<Question>().Where(w => w.Course.Id == courseName).UpdateBuilder()
               .Set(s => s.Course, newCourse).UpdateAsync(token);

            await Session.Query<Document>().Where(w => w.Course.Id == courseName).UpdateBuilder()
               .Set(s => s.Course, newCourse).UpdateAsync(token);



            var courseToDelete = await LoadAsync(courseName, token);
            await DeleteAsync(courseToDelete, token);
            newCourse.Approve();
        }
    }
}
