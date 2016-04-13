﻿
namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Box
    {
        //TODO: performance optimization check
        public const string BoxData = @"
select 
  b.BoxName as Name
 ,b.CourseCode as CourseId
 ,b.ProfessorName as ProfessorName
 ,b.ItemCount as Items
 ,b.MembersCount as Members
 ,b.commentCount as feeds
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
,itemtabname as name, itemCount as count
 from zbox.ItemTab
where boxid = @BoxId
order by name;";


       
        public const string GetBoxComments = @"SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	,u.UserImageLarge as UserImage
	,u.userid as UserId
    ,u.url as Url
    ,[Text] as Content
    ,q.CreationTime as creationTime
	,q.ReplyCount as RepliesCount
	,q.LikeCount as LikesCount
	FROM [Zbox].[Question] q join zbox.users u on u.userid = q.userid
    where q.BoxId = @BoxId
    and q.CreationTime < @TimeStamp
    order by q.[updatetime] desc
    offset @pageNumber*@rowsperpage ROWS
	FETCH NEXT @rowsperpage ROWS ONLY;";

        //used for reply view in mobile app
        public const string GetCommentForMobile = @" SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	  ,u.UserImageLarge as UserImage
	  ,u.userid as UserId
    ,[Text] as Content
    ,q.CreationTime as creationTime
	,q.ReplyCount as RepliesCount
    FROM [Zbox].[Question] q join zbox.users u on u.userid = q.userid
    where q.QuestionId = @QuestionId;
";
        public const string GetCommentFileForMobile = @"select
    i.itemid as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    i.AnswerId as AnswerId,
    i.Discriminator as Type,
    i.BlobName as Source
    from zbox.item i
    where i.IsDeleted = 0
    and i.BoxId = @BoxId
	and i.QuestionId = @QuestionId  
";

        public const string GetLastReplyOfComment = @"with questions as (
select questionid,LastReplyId from zbox.question
 where boxid = @boxid 
 and CreationTime < @TimeStamp
	            order by updatetime desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY
)
SELECT  a.[AnswerId] as id
	  ,u.[UserName] as UserName
      ,u.UserImageLarge as UserImage
      ,u.userid as UserId
      ,u.url as Url
      ,a.[Text] as Content
      ,a.LikeCount as LikesCount
      ,a.QuestionId as questionId
      ,a.CreationTime as creationTime
      FROM [Zbox].[Answer] a join questions q on a.AnswerId = q.LastReplyId
	  join zbox.users u on u.userid = a.userid;";

        public const string GetItemsForCommentsAndLastReply = @"with questions as (
select questionid,LastReplyId from zbox.question
 where boxid = @boxid 
 and CreationTime < @TimeStamp
	            order by updatetime desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY
)
 select
    i.itemid as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    i.AnswerId as AnswerId,
    i.Discriminator as Type,
    i.BlobName as Source,
	i.Url as Url
    from zbox.item i cross apply (select * from questions q
	             where i.QuestionId = q.QuestionId or (LastReplyId is not null and i.AnswerId = LastReplyId) 
				
				) t
    where i.IsDeleted = 0
    and i.BoxId = @BoxId";

        public const string GetQuizzesForCommentsAndLastReply = @"    select
    i.Id as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    i.url as Url,
    'quiz' as Type
    from zbox.Quiz i
    where i.IsDeleted = 0
	and i.Publish = 1
    and i.BoxId = @BoxId
    and QuestionId in  (select questionid from zbox.question where boxid = @boxid 
                and CreationTime < @TimeStamp
	            order by updatetime desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY)";

        public const string GetCommentRepliesInMobile = @" SELECT  a.[AnswerId] as id
	  ,u.[UserName] as UserName
      ,u.UserImageLarge as UserImage
      ,u.userid as UserId
      ,[Text] as Content
      ,a.CreationTime as creationTime
      ,a.LikeCount as LikesCount
      FROM [Zbox].[Answer] a 
	  join zbox.users u on u.userid = a.userid
	  where a.questionid = @CommentId
      and a.boxid =  @BoxId
	  order by id desc
	   offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY;";

        public const string GetCommentRepliesItemsInMobile = @" select
    i.itemid as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    i.AnswerId as AnswerId,
    i.Discriminator as Type,
    i.BlobName as Source
    from zbox.item i
    where i.IsDeleted = 0
    and i.BoxId = @BoxId
    and answerid in ( select answerid FROM [Zbox].[Answer] a 
	  where a.questionid = @CommentId 
	  order by answerid desc
	   offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY);";
      

        public const string RecommendedCourses = @"
select top 3 b.boxid, b.BoxName as Name,b.CourseCode,b.ProfessorName as professor,
b.MembersCount,b.ItemCount , b.url,count(*)  as x
from zbox.userboxrel ub join zbox.box b on ub.boxid = b.boxid and b.isdeleted = 0 and b.discriminator = 2
where userid in (
select userid from zbox.userboxrel where boxid = @BoxId)
and b.boxid <> @BoxId
and b.discriminator = 2
and b.university = (select university from zbox.box where boxid = @BoxId)
and @UserId not in (select ub2.userid from zbox.UserBoxRel ub2 where ub2.BoxId = b.BoxId)
group by b.boxid, b.BoxName ,b.CourseCode,b.ProfessorName ,
b.MembersCount,b.ItemCount , b.url
order by x desc;
";

        public const string LeaderBoard = @"
select top(3) u.userid as id, u.UserImageLarge as image, coalesce(u.FirstName,u.username) as name, u.UserReputation as score, u.Url as Url
from zbox.userboxrel ub 
join zbox.users u on ub.userid = u.userid
 where boxid = @BoxId
 and u.usertype <> 1
 and ub.UserType > 1
 and u.userreputation > 0
 order by userreputation desc;";




        public const string BoxMembers =
            @"  select u.UserId as Id , u.UserName as Name,u.UserImageLarge as Image , ub.UserType as userStatus, u.url as Url , null as Email
    from zbox.UserBoxRel ub 
	    join zbox.users u on ub.UserId =  u.UserId
	    where ub.BoxId=@BoxId
union 
	  select null as Id , m.UserName as Name,m.Image as Image , 1 as userStatus , null as Url, m.email
	  from zbox.invite m
	  where m.BoxId = @BoxId
	   and m.userboxrelid is null
       and m.isused = 0
	   order by userStatus desc
	offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";


        public const string BoxMembersWithoutInvited = @" select top (@top) u.UserId as Id , u.UserImageLarge as Image, u.UserReputation
    from zbox.UserBoxRel ub 
	    join zbox.users u on ub.UserId =  u.UserId
	    where ub.BoxId=@BoxId
		order by u.UserReputation desc";

        //TODO: improve the query
        public const string Items = @"
    select
    i.itemid as Id,
    i.Name as Name,
    i.userid as OwnerId,
    u.UserName as Owner,
    u.Url as UserUrl,
    i.Discriminator as Discriminator,
    i.ItemTabId as TabId,
    i.NumberOfViews as NumOfViews,
    i.LikeCount as Likes,
    i.sponsored as Sponsored,
    i.content as Description,
    i.NumberOfDownloads as NumOfDownloads,
    i.creationTime as Date,
	i.numberofcomments as commentsCount,
    i.Url as Url,
    i.Discriminator as type,
    i.BlobName as source
    from zbox.item i join zbox.users u on i.UserId = u.UserId
    where i.IsDeleted = 0
    and i.BoxId = @BoxId
    AND (@TabId IS null OR i.ItemTabId = @tabid)
    order by i.itemid desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";


        public const string ItemsWithoutTabs = @" select
    i.itemid as Id,
    i.Name as Name,
    i.userid as OwnerId,
    u.UserName as Owner,
    u.Url as UserUrl,
    i.Discriminator as Discriminator,
    i.NumberOfViews as NumOfViews,
    i.LikeCount as Likes,
    i.sponsored as Sponsored,
    i.BlobName as BlobName,
    i.NumberOfDownloads as NumOfDownloads,
    i.creationTime as Date,
	i.numberofcomments as commentsCount,
    i.Url as Url,
    i.Discriminator as type,
    i.BlobName as source
    from zbox.item i join zbox.users u on i.UserId = u.UserId
    where i.IsDeleted = 0
    and i.BoxId = @BoxId
    AND i.ItemTabId is null
    order by i.itemid desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";
    }


   
}
