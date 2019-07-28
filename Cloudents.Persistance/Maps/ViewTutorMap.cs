using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ViewTutorMap : ClassMap<ViewTutor>
    {
        public ViewTutorMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Image);
            Map(x => x.Subjects).CustomType<StringAggMapping>();
            Map(x => x.Courses).CustomType<StringAggMapping>();
            Map(x => x.CourseCount);
            Map(x => x.Price);
            Map(x => x.Rate);
            Map(x => x.SumRate);
            Map(x => x.Bio);
            Map(x => x.University);
            Map(x => x.Lessons);
            ReadOnly();
            Table("vTutorRead");
            SchemaAction.Validate();
        }

        /*create or alter view sb.vTutorRead as

Select 
t.id as id,
u.name,
u.image,
	(select STRING_AGG(cs.Name,',') 
	from sb.CourseSubject cs where cs.Id 
	in (
	select top 3 c.SubjectId from sb.Course c where c.SubjectId <> 39 
	and c.Name in (
	select uc.CourseId from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id )
	)) as subjects,
course.CourseId as Courses,
course.courseCount,
t.Price,
reviews.Rate ,
reviews.sumCount as SumRate,
t.bio,
u2.Name as University,
sr.lessonsCount as Lessons
from sb.tutor t
join sb.[user] u on t.id = u.id
left join sb.University u2 on u.UniversityId2 = u2.Id
cross apply (
select  STRING_AGG(t2.courseId,',') as CourseId, courseCount  from 
	(Select top 3 uc.courseid ,COUNT(*) OVER()  as courseCount
   from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id ) t2
   group by courseCount
) as course 
cross apply (
select Avg(tr.Rate) as Rate,count(*) as sumCount from sb.TutorReview tr where tr.TutorId = t.id 
) as reviews
cross apply (
select count(*) as lessonsCount from sb.StudyRoomSession srs 
join sb.StudyRoom sr on srs.StudyRoomId  = sr.id  and srs.Duration > 6000000000 and sr.TutorId = t.id
) as sr ;*/
    }
}