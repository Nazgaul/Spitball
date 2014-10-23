
namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Box
    {
        public const string BoxData = @"
select 
  b.PictureUrl as Image
 ,b.BoxName as Name
 ,b.CourseCode as CourseId
 ,b.ProfessorName as ProfessorName
 ,b.ItemCount as Items
 ,b.MembersCount as Members
 ,b.PrivacySetting
 ,b.quizcount as Quizes
 ,b.OwnerId as OwnerId
 ,b.Discriminator as BoxType
 ,b.CreationTime as Date
 ,(CASE 
      WHEN b.Discriminator is null THEN (select username from zbox.Users where userid = b.ownerid)
      Else (select universityname from zbox.university where id = b.university) 
   END ) as OwnerName
from zbox.box b
where b.BoxId = @BoxId
and b.IsDeleted = 0;
";
        public const string BoxTabs = @"
select 
ItemTabId as id
,itemtabname as name
 from zbox.ItemTab
where boxid = @BoxId;";


        public const string GetBoxQuestion = @"
 SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	  ,u.UserImage as UserImage
	  ,u.userid as UserId
    ,u.Url as Url
    ,[Text] as Content
    ,q.CreationTime as creationTime
    FROM [Zbox].[Question] q join zbox.users u on u.userid = q.userid
    where q.BoxId = @BoxId
    order by id desc;";

        public const string GetBoxAnswers = @" SELECT a.[AnswerId] as id
	    ,u.[UserName] as UserName
      ,u.UserImage as UserImage
      ,u.userid as UserId
      ,u.Url as Url
      ,[Text] as Content
      ,[QuestionId] as questionId
      ,a.CreationTime as creationTime
      FROM [Zbox].[Answer] a join zbox.users u on u.userid = a.userid
      where a.boxid = @BoxId
      order by MarkAnswer desc, id;";

        public const string GetBoxQnAItem = @"  select
    i.itemid as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.thumbnailurl as Thumbnail,
    i.QuestionId as QuestionId,
    i.AnswerId as AnswerId,
    i.Url as Url
    from zbox.item i
    where i.IsDeleted = 0
    and i.BoxId = @BoxId
    and (QuestionId is not null or AnswerId is not null);";
    }
}
