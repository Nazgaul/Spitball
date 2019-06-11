using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Criterion;

namespace Cloudents.Persistence.Repositories
{
    public class TutorRepository : NHibernateRepository<Tutor>, ITutorRepository
    {
        public TutorRepository(ISession session) : base(session)
        {
        }

        public async Task<IEnumerable<long>> GetTutorsByCourse(string course, long userId)
        {
            /*
             * Select * from sb.[tutor] t
join sb.[user] u on t.id = u.Id
where exists (select uc.* from sb.UsersCourses uc 
join sb.Course c on uc.CourseId = c.Name and c.State = 'Ok'
where u.Id = uc.UserId and uc.CanTeach = 1 and uc.CourseId = @CourseId
						union 
						select uc.* from sb.UsersCourses uc 
						join sb.Course c on uc.CourseId = c.Name and c.State = 'Ok'
						and c.SubjectId = (Select subjectId from sb.Course where Name = @CourseId)
						where u.Id = uc.UserId and uc.CanTeach = 1)
and not exists (select * from sb.ChatUser cu where cu.ChatRoomId in
(select ChatRoomId from sb.ChatUser where userid = @userid)
and not cu.UserId = @userid
and not cu.UserId = u.Id
)
and T.State = 'Ok'

             */
            Tutor tutorAlias = null;
            Course courseAlias = null;
            UserCourse userCourse = null;

            var chatRoomQuery = QueryOver.Of<ChatUser>()
                .WithSubquery.WhereProperty(w => w.ChatRoom.Id).In(
                    QueryOver.Of<ChatUser>().Where(w => w.User.Id == userId).Select(s=>s.ChatRoom.Id))
                .Where(w => w.User.Id != userId)
                .And(w => w.User.Id != tutorAlias.Id)
                .Select(s=>s.Id);

            var courseQuery = QueryOver.Of<UserCourse>(() => userCourse)
                    .JoinAlias(x => x.Course, () => courseAlias)
                    .Where(w => w.CanTeach)
                    .And(() => courseAlias.State == ItemState.Ok)
                    //.And(() => courseAlias.Id == course)
                    .And(Restrictions.Disjunction()
                        .Add(() => courseAlias.Id == course)
                        .Add(
                            Restrictions.EqProperty(
                                Projections.Property(() => courseAlias.Subject),
                                Projections.SubQuery(
                                    QueryOver.Of<Course>().Where(cs => cs.Id == course).Select(s=>s.Subject))))
                    )
                
                    .And(x => x.User.Id == tutorAlias.Id)
                    .Select(s => s.CanTeach)

                ;



            return await Session.QueryOver<Tutor>(() => tutorAlias)
                .JoinQueryOver(p => p.User)
                .WithSubquery.WhereExists(courseQuery)
                .WithSubquery.WhereNotExists(chatRoomQuery)
                .Select(s => s.User.Id).ListAsync<long>();
        }
    }
}