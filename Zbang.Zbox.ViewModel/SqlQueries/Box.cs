
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

        public const string RecommendedCourses = @"
select top 3 b.boxid, b.BoxName as Name,b.CourseCode,b.ProfessorName as professor,
b.PictureUrl as Picture,b.MembersCount,b.ItemCount , b.url,count(*)  as x
from zbox.userboxrel ub join zbox.box b on ub.boxid = b.boxid and b.isdeleted = 0 and b.discriminator = 2
where userid in (
select userid from zbox.userboxrel where boxid = @BoxId)
and b.boxid <> @BoxId
and @UserId not in (select ub2.userid from zbox.UserBoxRel ub2 where ub2.BoxId = b.BoxId)
group by b.boxid, b.BoxName ,b.CourseCode,b.ProfessorName ,
b.PictureUrl ,b.MembersCount,b.ItemCount , b.url
order by x desc;
";

        public const string LeaderBoard = @"
select top(5) u.userid as id, u.UserImageLarge as image, u.username as name, u.UserReputation as score
from zbox.userboxrel ub 
join zbox.users u on ub.userid = u.userid
 where boxid = @BoxId
 and u.usertype <> 1
 and ub.UserType > 1
 and u.userreputation > 0
 order by userreputation desc;";


        public const string BoxMembers =
            @"  select u.UserId as Id , u.UserName as Name,u.UserImage as Image , ub.UserType as userStatus, u.url as Url , null as Email
    from zbox.UserBoxRel ub 
	    join zbox.users u on ub.UserId =  u.UserId
	    where ub.BoxId=@BoxId
union 
	  select null as Id ,m .UserName as Name,m.Image as Image , 1 as userStatus , null as Url, m.email
	  from zbox.invite m
	  where m.BoxId = @BoxId
	   and m.userboxrelid is null
       and m.isused = 0
	   order by userStatus desc;";
    }
}
