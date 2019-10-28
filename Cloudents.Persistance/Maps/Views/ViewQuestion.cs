using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps.Views
{
    public class ViewQuestion : ClassMap<ViewQuestionWithFirstAnswer>
    {
        public ViewQuestion()
        {
            Id(x => x.Id);
            Map(x => x.Text);
            Map(x => x.Course);
            Map(x => x.Answers);
            Map(x => x.DateTime);
            Map(x => x.CultureInfo);
            //Map(x => x.UserId);
            Component(x => x.User, z => 
                { 
                    z.Map(x => x.Id).Column("User_Id");
                    z.Map(x => x.Name).Column("User_Name");
                    z.Map(x => x.Score).Column("User_Score");
                    z.Map(x => x.Image).Column("User_Image");
                }
            );
            Component(x => x.Answer, z =>
                {
                    z.Map(x2 => x2.UserId).Column("Answer_UserId");
                    z.Map(x2 => x2.UserImage).Column("Answer_UserImage");
                    z.Map(x2 => x2.UserName).Column("Answer_UserName");
                    z.Map(x2 => x2.Text).Column("Answer_Text");
                    z.Map(x2 => x2.DateTime).Column("Answer_DateTime");

                }
            );
            ReadOnly();
            Table("vQuestionCard");
        }

        /*
         * create or alter view sb.vQuestionCard as 
select q.Id as Id,
q.Text as Text,
q.CourseId as Course,
(SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE(this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
q.Updated as DateTime, 
q.Language as CultureInfo
,firstAnswer.Id as 'Answer.UserId'
,firstAnswer.Image as 'Answer.UserImage'
,firstAnswer.Name as 'Answer.UserName'
,firstAnswer.Text as 'Answer.Text'
,firstAnswer.Created as 'Answer.DateTime'
,q.userid as UserId

FROM sb.[Question] q 
outer apply (
select top 1 text, u.id, u.name, u.image, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
where a.QuestionId = q.Id and state = 'Ok' order by a.id) as firstAnswer
where q.State = 'Ok'
         */
    }
}