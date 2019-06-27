using System.Collections.Generic;
using System.Threading;
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

        public async Task<IList<long>> GetTutorsByCourseAsync(string course, long userId, CancellationToken token)
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
and cu.UserId = u.Id
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
                .And(w => w.User.Id == tutorAlias.Id)
                .Select(s=>s.Id);

            var courseQuery = QueryOver.Of(() => userCourse)
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



            return await Session.QueryOver(() => tutorAlias)
                .JoinQueryOver(p => p.User)
                .WithSubquery.WhereExists(courseQuery)
                .WithSubquery.WhereNotExists(chatRoomQuery)
                .And(x => x.Id != userId)
                .Select(s => s.User.Id)
                .Take(3)
                .ListAsync<long>(token);
        }

        public async Task DeleteTutorAsync(long tutorId, CancellationToken token)
        {
            const string sql = @"
begin tran
declare @studyroomId uniqueidentifier;

DECLARE studyroom_cursor CURSOR FOR   
select studyroomId  from sb.studyroomUser where UserId = :Userid;  

OPEN studyroom_cursor  
  
FETCH NEXT FROM studyroom_cursor   
INTO @studyroomId
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
  PRINT 'Delete study rooms '  
delete from sb.studyroomUser where studyroomId = @studyroomId
delete from sb.StudyRoomSession where studyroomId  = @studyroomId
delete from sb.StudyRoomSession where studyroomId  = @studyroomId
delete from sb.TutorReview where RoomId = @studyroomId
delete from sb.studyroom where id =@studyroomId
 FETCH NEXT FROM studyroom_cursor INTO @studyroomId  
END   
CLOSE studyroom_cursor; 
DEALLOCATE studyroom_cursor;  
 
--delete studyrooms



DECLARE chat_cursor CURSOR FOR   
select ChatRoomId  from sb.ChatUser where UserId = :Userid;  

OPEN chat_cursor  
  
FETCH NEXT FROM chat_cursor   
INTO @studyroomId
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
PRINT 'Delete chats '  
delete from sb.ChatMessage where ChatRoomId = @studyroomId
delete from sb.ChatUser where ChatRoomId  = @studyroomId
delete from sb.ChatRoomAdmin where Id  = @studyroomId
delete from sb.chatroom where Id  = @studyroomId
 FETCH NEXT FROM chat_cursor INTO @studyroomId  
END   
CLOSE chat_cursor; 
DEALLOCATE chat_cursor;


--delete can teach
PRINT 'upate courses '  
update sb.UsersCourses
set CanTeach = 0
where userid = :Userid
--delete tutor

PRINT 'delete tutor '  
update sb.Lead set TutorId = null where TutorId = :Userid
delete from sb.TutorReview where TutorId = :Userid
delete from sb.tutor where id = :Userid;

commit;";
            
           await Session.CreateSQLQuery(sql).SetParameter("Userid", tutorId).ExecuteUpdateAsync(token);
        }
    }
}