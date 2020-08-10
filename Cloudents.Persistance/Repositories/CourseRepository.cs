using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
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

        public  Task<Course?> GetCourseByNameAsync(long tutorId, string name, CancellationToken token)
        {
            return Session.Query<Course>()
                .Where(w => w.Name == name && w.Tutor.Id == tutorId)
                .SingleOrDefaultAsync(token);
        }

        public override async Task DeleteAsync(Course entity, CancellationToken token)
        {
            await RemoveOldObjectsAsync(entity, token);
            await base.DeleteAsync(entity, token);
        }

        private async Task RemoveOldObjectsAsync(Course entity, CancellationToken token)
        {
            var studyRooms = await entity.StudyRooms.AsQueryable().Select(s => s.Id).ToListAsync(cancellationToken: token);

            if (studyRooms.Count > 0)
            {
                var sqlQuery =
                    Session.CreateSQLQuery("delete from sb.UserToken where StudyRoomId in (:Id)");
                sqlQuery.SetParameterList("Id", studyRooms);

                await sqlQuery.ExecuteUpdateAsync(token);
            }
        }

        public override async Task UpdateAsync(Course entity, CancellationToken token)
        {
            await RemoveOldObjectsAsync(entity, token);
            await base.UpdateAsync(entity, token);
        }
        //public async Task MigrateCourseAsync(string courseToKeepId, string courseToRemoveId, CancellationToken token)
        //{
        //    var courseToKeep = await LoadAsync(courseToKeepId, token);

        //    await Session.Query<Document>().Where(w => w.Course.Id == courseToRemoveId).UpdateBuilder()
        //       .Set(s => s.Course, courseToKeep).UpdateAsync(token);

        //    await Session.Query<Question>().Where(w => w.Course.Id == courseToRemoveId).UpdateBuilder()
        //       .Set(s => s.Course, courseToKeep).UpdateAsync(token);


        //    var subQuery = Session.Query<UserCourse>().Where(w => w.Course.Id == courseToKeepId)
        //        .Select(s => s.User.Id);

        //    await Session.Query<UserCourse>()
        //        .Where(w => w.Course.Id == courseToRemoveId)
        //        .Where(w => !subQuery.Contains(w.User.Id))
        //        .UpdateBuilder()
        //        .Set(s => s.Course, courseToKeep)
        //        .UpdateAsync(token);

        //    await Session.Query<UserCourse>().Where(w => w.Course.Id == courseToRemoveId).DeleteAsync(token);
        //    var courseToRemove = await LoadAsync(courseToRemoveId, token);
        //    await DeleteAsync(courseToRemove, token);
        //}

        //public async Task RenameCourseAsync(string courseName, string newName, CancellationToken token)
        //{
        //    var newCourse = new Course(newName);
        //    await AddAsync(newCourse, token);
        //    await Session.FlushAsync(token);

        //    await Session.Query<UserCourse>().Where(w => w.Course.Id == courseName).UpdateBuilder()
        //       .Set(s => s.Course, newCourse).UpdateAsync(token);


        //    await Session.Query<Question>().Where(w => w.Course.Id == courseName).UpdateBuilder()
        //       .Set(s => s.Course, newCourse).UpdateAsync(token);

        //    await Session.Query<Document>().Where(w => w.Course.Id == courseName).UpdateBuilder()
        //       .Set(s => s.Course, newCourse).UpdateAsync(token);



        //    var courseToDelete = await LoadAsync(courseName, token);
        //    await DeleteAsync(courseToDelete, token);
        //    //newCourse.Approve();
        //}
    }
}
