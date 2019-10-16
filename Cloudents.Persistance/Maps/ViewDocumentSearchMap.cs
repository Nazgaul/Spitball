using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ViewDocumentSearchMap : ClassMap<ViewDocumentSearch>
    {
        public ViewDocumentSearchMap()
        {
            Id(x => x.Id);
            Map(x => x.University);
            Map(x => x.Course);
            Map(x => x.Snippet);
            Map(x => x.Professor);
            Map(x => x.Type);
            Map(x => x.Title);
            Map(x => x.UserId).Column("User_Id");
            Map(x => x.UserName).Column("User_Name");
            Map(x => x.UserScore).Column("User_Score");
            Map(x => x.UserImage).Column("User_Image");
            Map(x => x.Views);
            Map(x => x.Downloads);
            Map(x => x.DateTime);
            Map(x => x.Votes).Column("Vote_Votes");
            Map(x => x.Price);
            Map(x => x.UniversityId);
            Map(x => x.Country);
            Map(x => x.IsTutor).Column("User_IsTutor");
            Map(x => x.Purchased);
            Map(x => x.DocumentType).Column("DocumentType");
            Map(x => x.Duration);
            SchemaAction.Validate();
            Table("iv_DocumentSearch");
            ReadOnly();
        }
    }

//    public class DocumentAggregate : AbstractAuxiliaryDatabaseObject
//    {
        
//        public override string SqlCreateString(Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
//        {
//            return @"CREATE Or Alter VIEW [sb].[iv_DocumentSearch]
//WITH SCHEMABINDING  
//AS   
//    select 
//d.Id
//,un.Name as University
//,d.CourseName as Course
//,d.MetaContent as Snippet
//,d.Professor
//,d.Type
//,d.Name as Title
//,u.Id as User_Id
//,U.Name as User_Name
//,u.Score as User_Score
//,u.Image as User_Image
//,d.[Views]
//,d.Downloads
//,d.UpdateTime as [DateTime]
//,d.VoteCount as Vote_Votes
//,d.Price as Price
//,d.DocumentType as documentType
//,d.duration as Duration
//,(select count(1) from sb.[Transaction] where DocumentId = d.Id and [Action] = 'SoldDocument') as Purchased
//,un.Id as UniversityId
//,un.Country as Country,
//case when t.Id is null then cast(0 as bit) else cast(1 as bit) end as User_IsTutor

//from sb.Document d 
//join sb.[user] u on d.UserId = u.Id
//join sb.University un on un.Id = d.UniversityId
//left join sb.Tutor t
//	on t.Id = u.Id
//where d.State = 'Ok'
//GO";
//        }

//        public override string SqlDropString(Dialect dialect, string defaultCatalog, string defaultSchema)
//        {
//            return null;
//            //throw new System.NotImplementedException();
//        }
//    }
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
