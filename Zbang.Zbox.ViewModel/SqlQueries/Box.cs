
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
 ,b.LibraryId as DepartmentId
 ,(CASE 
      WHEN b.Discriminator is null THEN (select username from zbox.Users where userid = b.ownerid)
      Else (select universityname from zbox.university where id = b.university) 
   END ) as OwnerName
from zbox.box b
where b.BoxId = @BoxId
and b.IsDeleted = 0;";
        public const string BoxTabs = @"
select 
ItemTabId as id
,itemtabname as name, itemCount as count
 from zbox.ItemTab
where boxid = @BoxId
order by name;";

        //used for reply view in mobile app
        public const string GetCommentForMobile = @" SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	  ,u.UserImageLarge as UserImage
	  ,u.userid as UserId
    ,[Text] as Content
    ,q.CreationTime as creationTime
	,q.ReplyCount as RepliesCount
    FROM [Zbox].[Question] q join zbox.users u on u.userid = q.userid
    where q.QuestionId = @QuestionId;";

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

        public const string LeaderBoard = @"
with leaderboard as (
select u.userid as id,username as name,score,UserImageLarge as image,
ROW_NUMBER () over (partition BY boxid order by score desc) as location
 from zbox.Users  u 
 join zbox.UserBoxRel ub on u.UserId = ub.UserId and ub.boxid = @BoxId
 )
 SELECT *
FROM leaderboard
where location < 11 or id = @UserId";

        public const string BoxMembers =
            @"select u.UserId as Id , u.UserName as Name,u.UserImageLarge as Image , ub.UserType as userStatus, u.BadgeCount as badges, u.Score as score
    from zbox.UserBoxRel ub 
	    join zbox.users u on ub.UserId =  u.UserId
	    where ub.BoxId=@BoxId
	   order by userStatus desc
	offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        public const string BoxMembersWithoutInvited = @" select top (@top) u.UserId as Id , u.UserImageLarge as Image, u.Score
    from zbox.UserBoxRel ub 
	    join zbox.users u on ub.UserId =  u.UserId
	    where ub.BoxId=@BoxId
		order by u.Score desc";

        //TODO: improve the query
        public const string Items = @"
    select
    i.itemid as Id,
    i.Name as Name,
    i.userid as OwnerId,
    u.UserName as Owner,
    i.Discriminator as Discriminator,
    i.ItemTabId as TabId,
    i.NumberOfViews as NumOfViews,
    i.LikeCount as Likes,
    i.content as Description,
    i.NumberOfDownloads as NumOfDownloads,
    i.creationTime as Date,
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

        //TODO improve query
        public const string ItemsWithoutTabs = @" select
    i.itemid as Id,
    i.Name as Name,
    i.userid as OwnerId,
    u.UserName as Owner,
    i.Discriminator as Discriminator,
    i.NumberOfViews as NumOfViews,
    i.LikeCount as Likes,
    i.BlobName as BlobName,
    i.NumberOfDownloads as NumOfDownloads,
    i.creationTime as Date,
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

        public const string Flashcards = @" select 
    id as Id, 
    f.userid as OwnerId,
    name as Name,
    Publish,
    LikeCount as likes,
    NumberOfViews as NumOfViews
    from zbox.Flashcard f
    where boxid = @BoxId
    and f.isdeleted = 0
	order by f.Id desc
	";
    }
}
