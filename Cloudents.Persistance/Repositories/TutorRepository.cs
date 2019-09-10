using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Persistence.Repositories
{
    public class TutorRepository : NHibernateRepository<Tutor>, ITutorRepository
    {
        public TutorRepository(ISession session) : base(session)
        {
        }

        public override Task DeleteAsync(Tutor entity, CancellationToken token)
        {
            entity.Delete();
            return base.DeleteAsync(entity, token);
        }

        public async Task<IList<long>> GetTutorsByCourseAsync(string course, long userId, string country, CancellationToken token)
        {
            const string sql = @"with cte as
(
select distinct uc.UserId, case when uc.UserId in (select UserId from sb.UsersCourses where CourseId = :Course and CanTeach = 1) then 1 else 0 end as IsMatch
from sb.Course c
join sb.UsersCourses uc
	on uc.CourseId = c.Name
		WHERE (c.SubjectId in (select SubjectId from sb.Course where Name = :Course) or c.Name = :Course) and CanTeach = 1 
			and exists (
						SELECT uc1.CanTeach
						FROM sb.UsersCourses uc1
						inner join sb.[Course] c 
						on uc1.CourseId=c.Name 
						WHERE uc1.CanTeach = 1 and c.[State] = 'Ok' 
						and (
							c.Name = :Course 
							or c.SubjectId = (SELECT c1.SubjectId as y0_ 
												FROM sb.[Course] c1 
												WHERE c1.Name =  :Course)
						)
						
					) 
		and uc.UserId not in (
						SELECT uc1.UserId 
						FROM sb.[ChatUser] uc1 
						WHERE uc1.ChatRoomId in (
													SELECT chu.ChatRoomId 
													FROM sb.[ChatUser] chu 
													WHERE chu.UserId = :UserId
												) 
		and uc1.UserId != :UserId 
		)
    and uc.UserId != :UserId 
)

select ts.Id--, IsMatch , case when cte.UserId is null then 0 else 1 end
from sb.vTutorSearch ts
join cte on cte.UserId = ts.Id
order by IsMatch desc, case when cte.UserId is null then 0 else 1 end desc,
ts.ResponseTimeScore + ts.LessonsDoneScore + ts.LastOnlineScore + case when ts.Country = :Country then 0 else -5 end + RateScore + ManualBoost desc
OFFSET 0 ROWS FETCH FIRST 3 ROWS ONLY";

var res =  await Session.CreateSQLQuery(sql).SetParameter("UserId", userId).SetParameter("Course", course)
                .SetParameter("Country", country).ListAsync<long>(token);
return res;
            /*
             * Select * from sb.[tutor] t
join sb.[user] u on t.id = u.Id
where exists (select uc.* from sb.UsersCourses uc 
join sb.Course c on uc.CourseId = c.Name and c.State = 'Ok'
where u.Id = uc.UserId and uc.CanTeach = 1 and uc.CourseId = :CourseId
						union 
						select uc.* from sb.UsersCourses uc 
						join sb.Course c on uc.CourseId = c.Name and c.State = 'Ok'
						and c.SubjectId = (Select subjectId from sb.Course where Name = :CourseId)
						where u.Id = uc.UserId and uc.CanTeach = 1)
and not exists (select * from sb.ChatUser cu where cu.ChatRoomId in
(select ChatRoomId from sb.ChatUser where userid = :UserId)
and not cu.UserId = :UserId
and cu.UserId = u.Id
)
and T.State = 'Ok'

             */
            //Tutor tutorAlias = null;
            //Course courseAlias = null;
            //UserCourse userCourse = null;

            //var chatRoomQuery = QueryOver.Of<ChatUser>()
            //    .WithSubquery.WhereProperty(w => w.ChatRoom.Id).In(
            //        QueryOver.Of<ChatUser>().Where(w => w.User.Id == userId).Select(s=>s.ChatRoom.Id))
            //    .Where(w => w.User.Id != userId)
            //    .And(w => w.User.Id == tutorAlias.Id)
            //    .Select(s=>s.Id);

            //var courseQuery = QueryOver.Of(() => userCourse)
            //        .JoinAlias(x => x.Course, () => courseAlias)
            //        .Where(w => w.CanTeach)
            //        .And(() => courseAlias.State == ItemState.Ok)
            //        .And(Restrictions.Disjunction()
            //            .Add(() => courseAlias.Id == course)
            //            .Add(
            //                Restrictions.EqProperty(
            //                    Projections.Property(() => courseAlias.Subject),
            //                    Projections.SubQuery(
            //                        QueryOver.Of<Course>().Where(cs => cs.Id == course).Select(s=>s.Subject))))
            //        )

            //        .And(x => x.User.Id == tutorAlias.Id)
            //        .Select(s => s.CanTeach)

            //    ;



            //return await Session.QueryOver(() => tutorAlias)
            //    .JoinQueryOver(p => p.User)
            //    .WithSubquery.WhereExists(courseQuery)
            //    .WithSubquery.WhereNotExists(chatRoomQuery)
            //    .And(() => tutorAlias.State == ItemState.Ok)
            //    .And(x => x.Id != userId)
            //    .Select(s => s.User.Id)
            //    .Take(3)
            //    .ListAsync<long>(token);
        }

//        public async Task DeleteTutorAsync(long tutorId, CancellationToken token)
//        {
//            const string sql = @"
//begin tran
//declare @studyRoomId uniqueidentifier;

//DECLARE studyroom_cursor CURSOR FOR   
//select studyRoomId  from sb.studyroomUser where UserId = :Userid;  

//OPEN studyroom_cursor  
  
//FETCH NEXT FROM studyroom_cursor   
//INTO @studyRoomId
  
//WHILE @@FETCH_STATUS = 0  
//BEGIN  
//  PRINT 'Delete study rooms '  
//delete from sb.studyroomUser where studyRoomId = @studyRoomId
//delete from sb.StudyRoomSession where studyRoomId  = @studyRoomId
//delete from sb.StudyRoomSession where studyRoomId  = @studyRoomId
//delete from sb.TutorReview where RoomId = @studyRoomId
//delete from sb.studyroom where id =@studyRoomId
// FETCH NEXT FROM studyroom_cursor INTO @studyRoomId  
//END   
//CLOSE studyroom_cursor; 
//DEALLOCATE studyroom_cursor;  
 
//--delete studyrooms



//DECLARE chat_cursor CURSOR FOR   
//select ChatRoomId  from sb.ChatUser where UserId = :Userid;  

//OPEN chat_cursor  
  
//FETCH NEXT FROM chat_cursor   
//INTO @studyRoomId
  
//WHILE @@FETCH_STATUS = 0  
//BEGIN  
//PRINT 'Delete chats '  
//delete from sb.ChatMessage where ChatRoomId = @studyRoomId
//delete from sb.ChatUser where ChatRoomId  = @studyRoomId
//delete from sb.ChatRoomAdmin where Id  = @studyRoomId
//delete from sb.chatroom where Id  = @studyRoomId
// FETCH NEXT FROM chat_cursor INTO @studyRoomId  
//END   
//CLOSE chat_cursor; 
//DEALLOCATE chat_cursor;


//--delete can teach
//PRINT 'upate courses '  
//update sb.UsersCourses
//set CanTeach = 0
//where userid = :Userid
//--delete tutor

//PRINT 'delete tutor '  
//update sb.Lead set TutorId = null where TutorId = :Userid
//delete from sb.TutorReview where TutorId = :Userid
//delete from [sb].[TutorCalendar] where TutorId = @Userid;
//delete from sb.tutor where id = :Userid;
//delete from sb.ReadTutor where id = :Userid;
//delete from sb.GoogleTokens where Id = :Userid
//commit;";
            
//           await Session.CreateSQLQuery(sql).SetParameter("Userid", tutorId).ExecuteUpdateAsync(token);
//        }
    }
}