using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;

namespace Cloudents.Persistence.Maps
{
    public class ViewTutorMap : ClassMapping<ViewTutor>
    {
        public ViewTutorMap()
        {
            Id(x => x.Id);
            //Id(x => x.Id);
            Property(x => x.Name);
            //Map(x => x.Name);
            Property(x => x.Image);
            //Map(x => x.Image);
            Property(x => x.Subjects, c => c.Type<StringAggMapping>());
            //Map(x => x.Subjects).CustomType<StringAggMapping>();
            Property(x => x.Courses, c => c.Type<StringAggMapping>());
            //Map(x => x.Courses).CustomType<StringAggMapping>();

            ////Map(x => x.CourseCount);
            Property(x => x.Price, c => c.Column(cl => cl.SqlType("smallmoney")));
            //Map(x => x.Price);
            Property(x => x.Rate, c => c.Column(cl => cl.SqlType("float")));
            //Map(x => x.Rate);
            Property(x => x.SumRate);
            //Map(x => x.SumRate);
            Property(x => x.Bio);
            //Map(x => x.Bio);
            Property(x => x.University);
            //Map(x => x.University);
            Property(x => x.Lessons);
            //Map(x => x.Lessons);
            Mutable(false);
            //ReadOnly();
            Table("vTutorRead");
            //Table("vTutorRead");
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();
        }

        /*alter     view [sb].[vTutorRead] as

Select 
t.id as id,
u.name,
u.image,
	(select top 3 cs.Name as 'name' 
	from sb.CourseSubject cs where cs.Id 
	in (
	select  c.SubjectId from sb.Course c where c.SubjectId <> 39 
	and c.Name in (
	select uc.CourseId as Courses from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id )
	) 
	order by name
	for json PATH) as subjects,
(Select top 3 uc.courseid as 'name' 
   from sb.UsersCourses uc where uc.CanTeach = 1 and uc.UserId = t.id for json PATH) as Courses,
t.Price,
reviews.Rate ,
reviews.sumCount as SumRate,
t.bio,
u2.Name as University,
case when sr.lessonsCount < reviews.sumCount then reviews.sumCount else sr.lessonsCount end as Lessons
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
where t.State = 'Ok';
GO*/
    }
}