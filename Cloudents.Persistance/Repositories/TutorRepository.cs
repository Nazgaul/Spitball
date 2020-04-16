using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
select distinct uc.UserId, case when uc.UserId in (select UserId from sb.UserCourse2 where CourseId = :Course) then 1 else 0 end as IsMatch
from sb.Course2 c
join sb.UserCourse2 uc
	on uc.CourseId = c.Id
		WHERE (c.Field in (select Field from sb.Course2 where SearchDisplay = :Course) or c.SearchDisplay = :Course) 
			and exists (
						SELECT uc1.*
						FROM sb.UserCourse2 uc1
						inner join sb.[Course2] c 
						on uc1.CourseId=c.Id 
						WHERE c.[State] = 'Ok' 
						and (
							c.SearchDisplay = :Course 
							or c.Field = (SELECT c1.Field as y0_ 
												FROM sb.[Course2] c1 
												WHERE c1.SearchDisplay =  :Course)
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