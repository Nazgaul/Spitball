using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;

namespace Cloudents.Persistence.Maps
{
    public class ViewDocumentSearchMap : ClassMapping<ViewDocumentSearch>
    {
        public ViewDocumentSearchMap()
        {
            Id(x => x.Id);
            //Id(x => x.Id);
            Property(x => x.University);
            //Map(x => x.University);
            Property(x => x.Course);
            //Map(x => x.Course);
            Property(x => x.Snippet);
            //Map(x => x.Snippet);
            Property(x => x.Professor);
            //Map(x => x.Professor);
            Property(x => x.Type);
            //Map(x => x.Type);
            Property(x => x.Title);
            //Map(x => x.Title);
            Property(x => x.UserId, c => c.Column("User_Id"));
            //Map(x => x.UserId).Column("User_Id");
            Property(x => x.UserName, c => c.Column("User_Name"));
            //Map(x => x.UserName).Column("User_Name");
            Property(x => x.UserScore, c => c.Column("User_Score"));
            //Map(x => x.UserScore).Column("User_Score");
            Property(x => x.UserImage, c => c.Column("User_Image"));
            //Map(x => x.UserImage).Column("User_Image");
            Property(x => x.Views);
            //Map(x => x.Views);
            Property(x => x.Downloads);
            //Map(x => x.Downloads);
            Property(x => x.DateTime);
            //Map(x => x.DateTime);
            Property(x => x.Votes, c => c.Column("Vote_Votes"));
            //Map(x => x.Votes).Column("Vote_Votes");
            Property(x => x.Price, c => c.Column(cl => cl.SqlType("smallmoney")));
            //Map(x => x.Price);
            Property(x => x.UniversityId);
            //Map(x => x.UniversityId);
            Property(x => x.Country);
            //Map(x => x.Country);
            Property(x => x.IsTutor, c => c.Column("User_IsTutor"));
            //Map(x => x.IsTutor).Column("User_IsTutor");
            Property(x => x.Purchased);
            //Map(x => x.Purchased);
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();
            Table("iv_DocumentSearch");
            //Table("iv_DocumentSearch");
            Mutable(false);
            //ReadOnly();
        }
    }
}


/*
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER VIEW[sb].[iv_DocumentSearch]
WITH SCHEMABINDING
AS
    select
d.Id
,un.Name as University
,d.CourseName as Course
,d.MetaContent as Snippet
,d.Professor
,d.Type
,d.Name as Title
,u.Id as User_Id
,U.Name as User_Name
,u.Score as User_Score
,u.Image as User_Image
,d.[Views]
,d.Downloads
,d.UpdateTime as [DateTime]
,d.VoteCount as Vote_Votes
--,(select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = cte.userid) as Vote_Vote
,d.Price as Price
,d.Purchased
,un.Id as UniversityId
,un.Country as Country,
case when t.Id is null then cast(0 as bit) else cast(1 as bit) end as Uer_IsTutor

from sb.Document d
join sb.[user] u on d.UserId = u.Id
join sb.University un on un.Id = d.UniversityId
left join sb.Tutor t

    on t.Id = u.Id
where d.State = 'Ok';
GO
*/
