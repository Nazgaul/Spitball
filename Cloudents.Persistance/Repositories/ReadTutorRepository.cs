using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class ReadTutorRepository: NHibernateRepository<ReadTutor>, IReadTutorRepository
    {
        public ReadTutorRepository(ISession session) : base(session)
        {
           
        }

        public async Task<ReadTutor> GetReadTutorAsync(long userId, CancellationToken token)
        {
            const string sql = @"
Select top 1
t.id as Id,
u.name as Name,
u.image as Image,
	(select cs.Name as 'name' 
	from sb.CourseSubject cs where cs.Id 
	in (
	select distinct top 3 c.SubjectId from sb.Course c where c.SubjectId <> 39 
	and c.Name in (
	select uc.CourseId as Courses from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id )
	) order by cs.Name for json PATH) as Subjects,

	(select cs.Name as 'name' 
	from sb.CourseSubject cs where cs.Id 
	in (
	select c.SubjectId from sb.Course c where c.SubjectId <> 39 
	and c.Name in (
	select uc.CourseId as Courses from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id )
	) order by cs.Name for json PATH) as AllSubjects,

(Select distinct top 3 uc.courseid as 'name' 
   from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id order by uc.courseid for json PATH) as Courses,
(Select uc.courseid as 'name' 
   from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id order by uc.courseid for json PATH) as AllCourses,
t.Price,
reviews.Rate ,
reviews.sumCount as RateCount,
t.bio as Bio,
u2.Name as University,
iif(sr.lessonsCount < reviews.sumCount, reviews.sumCount, sr.lessonsCount) as Lessons,
((isnull(reviews.Rate, 0) * reviews.sumCount) + (isnull((select AVG(Rate) from sb.TutorReview), 0) * 12) + 
(case when sr.lessonsCount < reviews.sumCount then reviews.sumCount else sr.lessonsCount end * isnull(reviews.Rate, 0))) / 
(reviews.sumCount + 12 + case when sr.lessonsCount < reviews.sumCount then reviews.sumCount else sr.lessonsCount end)as Rating
--,(isnull(reviews.Rate, 0) * reviews.sumCount) + (isnull((select AVG(Rate) from sb.TutorReview),0) * 12)
from sb.tutor t
join sb.[user] u on t.id = u.id
left join sb.University u2 on u.UniversityId2 = u2.Id
cross apply (
select Avg(tr.Rate) as Rate,count(*) as sumCount from sb.TutorReview tr where tr.TutorId = t.id 
) as reviews
cross apply (
select count(*) as lessonsCount from sb.StudyRoomSession srs 
join sb.StudyRoom sr on srs.StudyRoomId  = sr.id  and srs.Duration > 6000000000 and sr.TutorId = t.id
) as sr 
where t.State = 'Ok' and t.Id = :UserId";

            var info = await Session.CreateSQLQuery(sql)
                .SetParameter("UserId", userId).SetResultTransformer(new JsonArrayToEnumerableTransformer<ReadTutor>()).ListAsync<ReadTutor>();
        

            var res = info.First();
            return res;
        }


        public void UpdateReadTutorRating(CancellationToken token)
        {
            const string sql = @"update rt
                            set Rating = (select ((isnull(reviews.Rate, 0) * reviews.sumCount) +  (isnull((select AVG(Rate) from sb.TutorReview), 0) * 12)  + 
                            (case when sr.lessonsCount < reviews.sumCount then reviews.sumCount else sr.lessonsCount end * isnull(reviews.Rate, 0))) / 
                            (reviews.sumCount + 12 + case when sr.lessonsCount < reviews.sumCount then reviews.sumCount else sr.lessonsCount end) as Rating
				                            from sb.Tutor t 
                                            cross apply (
					                            select Avg(tr.Rate) as Rate,count(*) as sumCount from sb.TutorReview tr where tr.TutorId = t.id 
					                            ) as reviews
				                            cross apply (
				                            select count(*) as lessonsCount from sb.StudyRoomSession srs 
				                            join sb.StudyRoom sr on srs.StudyRoomId  = sr.id  and srs.Duration > 6000000000 and sr.TutorId = t.id
				                            ) as sr 
				                            where t.Id = rt.Id
			                            )
                            from sb.ReadTutor rt";
            try
            {
                Session.CreateSQLQuery(sql).ExecuteUpdate();
            }
            catch(Exception e)
            {

            }
        }
    }
}
