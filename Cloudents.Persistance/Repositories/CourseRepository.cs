using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class CourseRepository : NHibernateRepository<Course>, ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {

        }

        public async Task MigrateCourseAsync(string courseToKeepId, string courseToRemoveId, CancellationToken token)
        {
            var courseToKeep = await LoadAsync(courseToKeepId, token);

            var updateDocuments = await Session.Query<Document>().Where(w => w.Course.Id == courseToRemoveId).UpdateBuilder()
                .Set(s => s.Course, courseToKeep).UpdateAsync(token);

            var updateQuestions = await Session.Query<Question>().Where(w => w.Course.Id == courseToRemoveId).UpdateBuilder()
                .Set(s => s.Course, courseToKeep).UpdateAsync(token);


            var subQuery = Session.Query<UserCourse>().Where(w => w.Course.Id == courseToKeepId).Select(s => s.User.Id);

            var updateUsersCourses = await Session.Query<UserCourse>().Where(w => w.Course.Id == courseToRemoveId)
                .Where(w => !subQuery.Contains(w.User.Id))
                .UpdateBuilder()
                .Set(s => s.Course, courseToKeep).UpdateAsync(token);

            var deleteUsersCourses = await Session.Query<UserCourse>().Where(w => w.Course.Id == courseToRemoveId).DeleteAsync(token);
            var courseToRemove = await LoadAsync(courseToRemoveId, token);
            await DeleteAsync(courseToRemove, token);
        }

        public async Task RenameCourseAsync(string CourseName, string NewName, CancellationToken token)
        {
            var NewCourse = new Course(NewName);
            await AddAsync(NewCourse, token);

            var usersCourses = await Session.Query<UserCourse>().Where(w => w.Course.Id == CourseName).UpdateBuilder()
                .Set(s => s.Course, NewCourse).UpdateAsync(token);
         

            var updateQuestions = await Session.Query<Question>().Where(w => w.Course.Id == CourseName).UpdateBuilder()
                .Set(s => s.Course, NewCourse).UpdateAsync(token);

            var documents = await Session.Query<Document>().Where(w => w.Course.Id == CourseName).UpdateBuilder()
                .Set(s => s.Course, NewCourse).UpdateAsync(token);

        

            var courseToDelete = await LoadAsync(CourseName, token);
            await DeleteAsync(courseToDelete, token);
            NewCourse.Approve(null);
        }
    }
}
