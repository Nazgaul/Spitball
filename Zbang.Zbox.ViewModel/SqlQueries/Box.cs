
namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Box
    {
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
,itemtabname as name
 from zbox.ItemTab
where boxid = @BoxId
order by name;";


        public const string GetBoxQuestion = @"
 SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	  ,u.UserImageLarge as UserImage
	  ,u.userid as UserId
    ,u.Url as Url
    ,[Text] as Content
    ,q.CreationTime as creationTime
    FROM [Zbox].[Question] q join zbox.users u on u.userid = q.userid
    where q.BoxId = @BoxId
    order by q.[QuestionId] desc
    offset @pageNumber*@rowsperpage ROWS
	FETCH NEXT @rowsperpage ROWS ONLY;";

     

        public const string GetBoxAnswers = @"  SELECT a.[AnswerId] as id
	  ,u.[UserName] as UserName
      ,u.UserImageLarge as UserImage
      ,u.userid as UserId
      ,u.Url as Url
      ,[Text] as Content
      ,[QuestionId] as questionId
      ,a.CreationTime as creationTime
      FROM [Zbox].[Answer] a join zbox.users u on u.userid = a.userid
	  where a.boxid = @BoxId
	  and  a.questionid in 
            (select questionid from zbox.question where boxid = @boxid 
	            order by questionid desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY)
	  order by id;";

        public const string GetBoxQnAItem = @"  select
    i.itemid as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    i.AnswerId as AnswerId,
    i.Url as Url,
    i.Discriminator as Type,
    i.BlobName as Source
    from zbox.item i
    where i.IsDeleted = 0
    and i.BoxId = @BoxId
    and (QuestionId in  (select questionid from zbox.question where boxid = @boxid 
	            order by questionid desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY)
				or AnswerId is not null);";


        public const string GetBoxQnaQuiz = @"	select
    i.Id as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    i.Url as Url
    from zbox.Quiz i
    where i.Publish = 1
    and i.isdeleted = 0
    and i.BoxId = @BoxId
    and QuestionId in  (select questionid from zbox.question where boxid = @boxid 
	            order by questionid desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY);";

        #region mobileFeed
        public const string GetBoxCommentsForMobile = @" SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	  ,u.UserImageLarge as UserImage
	  ,u.userid as UserId
    ,[Text] as Content
    ,q.CreationTime as creationTime
	,(select count(*) from zbox.Answer where questionid = q.questionid and boxid = @BoxId) as RepliesCount
    FROM [Zbox].[Question] q join zbox.users u on u.userid = q.userid
    where q.BoxId = @BoxId
    order by q.[updatetime] desc
    offset @pageNumber*@rowsperpage ROWS
	FETCH NEXT @rowsperpage ROWS ONLY;";


        public const string GetCommentForMobile = @" SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	  ,u.UserImageLarge as UserImage
	  ,u.userid as UserId
    ,[Text] as Content
    ,q.CreationTime as creationTime
	,(select count(*) from zbox.Answer where questionid = q.questionid and boxid = @BoxId) as RepliesCount
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

        public const string GetLastCommentRepliesForMobile = @"with last_reply as (
select max([AnswerId]) as 'id', questionid  from [Zbox].[Answer] a 
where a.boxid = @BoxId 
	  and  a.questionid in 
            (select questionid from zbox.question where boxid = @BoxId
	            order by updatetime desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY
				)
				group by questionid
)
	 SELECT  a.[AnswerId] as id
	  ,u.[UserName] as UserName
      ,u.UserImageLarge as UserImage
      ,u.userid as UserId
      ,[Text] as Content
      ,last_reply.QuestionId as questionId
      ,a.CreationTime as creationTime
      FROM [Zbox].[Answer] a 
	  join zbox.users u on u.userid = a.userid
	  join last_reply on a.answerid = last_reply.id;";

        public const string GetBoxItemForCommentInMobile = @"select
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
    and (
		QuestionId in  (select questionid from zbox.question where boxid = @boxid 
	            order by updatetime desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY)
		or 
		AnswerId in (
				select max([AnswerId]) as 'id'  from [Zbox].[Answer] a 
				where a.boxid = @BoxId
				and  a.questionid in 
					(select questionid from zbox.question where boxid = @BoxId 
					order by updatetime desc
					offset @pageNumber*@rowsperpage ROWS
					FETCH NEXT @rowsperpage ROWS ONLY
				)
				group by questionid)
				)";

        public const string GetBoxQuizFromCommentInMobile = @"    select
    i.Id as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    --i.AnswerId as AnswerId,
    'Quiz' as Type
    --i.BlobName as Source
    from zbox.Quiz i
    where i.IsDeleted = 0
	and i.Publish = 1
    and i.BoxId = @BoxId
    and QuestionId in  (select questionid from zbox.question where boxid = @boxid 
	            order by updatetime desc
	            offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY)";

        public const string GetCommentRepliesInMobile = @" SELECT  a.[AnswerId] as id
	  ,u.[UserName] as UserName
      ,u.UserImageLarge as UserImage
      ,u.userid as UserId
      ,[Text] as Content
      ,a.CreationTime as creationTime
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
        #endregion

        public const string RecommendedCourses = @"
select top 3 b.boxid, b.BoxName as Name,b.CourseCode,b.ProfessorName as professor,
b.MembersCount,b.ItemCount , b.url,count(*)  as x
from zbox.userboxrel ub join zbox.box b on ub.boxid = b.boxid and b.isdeleted = 0 and b.discriminator = 2
where userid in (
select userid from zbox.userboxrel where boxid = @BoxId)
and b.boxid <> @BoxId
and @UserId not in (select ub2.userid from zbox.UserBoxRel ub2 where ub2.BoxId = b.BoxId)
group by b.boxid, b.BoxName ,b.CourseCode,b.ProfessorName ,
b.MembersCount,b.ItemCount , b.url
order by x desc;
";

        public const string LeaderBoard = @"
select top(5) u.userid as id, u.UserImageLarge as image, u.username as name, u.UserReputation as score, u.Url as Url
from zbox.userboxrel ub 
join zbox.users u on ub.userid = u.userid
 where boxid = @BoxId
 and u.usertype <> 1
 and ub.UserType > 1
 and u.userreputation > 0
 order by userreputation desc;";


        public const string BoxUserIds = @"select userid from zbox.userboxrel
where boxid = @BoxId
and userid != @UserId
    order by userid
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

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
    i.rate as Rate,
    i.sponsored as Sponsored,
    i.content as Description,
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
    AND (@TabId IS null OR i.ItemTabId = @tabid)
    order by i.itemid desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";
    }
}
