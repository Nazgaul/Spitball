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

        public async Task<IList<long>> GetTutorsByCourseAsync(string course, long userId, string country,
            CancellationToken token)
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
where ts.Country = :Country
order by IsMatch desc, case when cte.UserId is null then 0 else 1 end desc,
ts.ResponseTimeScore + ts.LessonsDoneScore + ts.LastOnlineScore + RateScore + ManualBoost desc
OFFSET 0 ROWS FETCH FIRST 3 ROWS ONLY";

            var res = await Session.CreateSQLQuery(sql).SetParameter("UserId", userId).SetParameter("Course", course)
                .SetParameter("Country", country).ListAsync<long>(token);
            return res;

        }
    }
}