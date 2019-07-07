using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ViewUserDetailMap : ClassMap<ViewUserDetail>
    {
        public ViewUserDetailMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Image);
            Map(x => x.Courses);
            Map(x => x.Price);
            Map(x => x.Bio);
            Map(x => x.Rate);
            Map(x => x.ReviewsCount);
            Map(x => x.Score);
            ReadOnly();
            Table("vUserDetail");
            SchemaAction.Validate();

            /*CREATE VIEW sb.vUserDetail AS
Select  U.Id as Id, U.Name, U.Image,u.score,
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price, 
T.Bio,
	                       x.*
                        from sb.[user] U
                        left join sb.Tutor T

	                        on U.Id = T.Id
				cross apply (select avg(Rate) as Rate, count(1) as ReviewsCount from sb.TutorReview where TutorId = T.Id) as x
*/
        }
    }
}