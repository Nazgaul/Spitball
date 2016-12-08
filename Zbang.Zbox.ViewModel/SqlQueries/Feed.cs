namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Feed
    {
        public const string Comments = @"SELECT q.[QuestionId] as id
    ,u.[UserName] as UserName
	,u.UserImageLarge as UserImage
	,u.userid as UserId
    ,u.BadgeCount as Badges
	,u.Score as Score
    ,[Text] as Content
    ,q.CreationTime as creationTime
	,q.ReplyCount as RepliesCount
	,q.LikeCount as LikesCount
	FROM [Zbox].[Question] q join zbox.users u on u.userid = q.userid
    where q.BoxId = @BoxId
    order by q.[updatetime] desc
    offset @skip ROWS
	FETCH NEXT @top ROWS ONLY;";


        public const string RepliesInComments = @"with questions as (
select questionid from zbox.question
 where boxid = @boxid 
 order by updatetime desc
 offset @skip ROWS
 FETCH NEXT @top ROWS ONLY
)
select a.[AnswerId] as id
	  ,u.[UserName] as UserName
      ,u.UserImageLarge as UserImage
      ,u.userid as UserId
      ,u.BadgeCount as Badges
	  ,u.Score as Score
      ,a.[Text] as Content
      ,a.LikeCount as LikesCount
      ,a.QuestionId as questionId
      ,a.CreationTime as creationTime 
 from questions q
cross apply (select top (@rtop) * from zbox.Answer a where a.QuestionId = q.QuestionId order by a.AnswerId desc) as a
join zbox.users u on u.userid = a.userid;";


        public const string GetItemsInCommentsAndReplies = @"with questions as (
select questionid from zbox.question
 where boxid = @boxid 
 order by updatetime desc
 offset @skip ROWS
 FETCH NEXT @top ROWS ONLY
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
	             where i.QuestionId = q.QuestionId 
				) t
    where i.IsDeleted = 0
    and i.BoxId = @BoxId

	union all
	select
    i.itemid as Id,
    i.Name as Name,
    i.UserId as OwnerId,
    i.QuestionId as QuestionId,
    i.AnswerId as AnswerId,
    i.Discriminator as Type,
    i.BlobName as Source,
	i.Url as Url
    from zbox.item i 
				cross apply (select top (@rtop) * from zbox.Answer a where i.AnswerId = a.AnswerId order by a.AnswerId desc) as z
    where i.IsDeleted = 0
    and i.BoxId = @BoxId;";


        public const string GetQuizzesForComments = @"select
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
                
	            order by updatetime desc
	            offset @skip ROWS
	FETCH NEXT @top ROWS ONLY)";


        public const string GetReplies = @" SELECT  a.[AnswerId] as id
	  ,u.[UserName] as UserName
      ,u.UserImageLarge as UserImage
      ,u.userid as UserId
,u.BadgeCount as Badges
	,u.Score as Score
      ,[Text] as Content
      ,a.CreationTime as creationTime
      ,a.LikeCount as LikesCount
      FROM [Zbox].[Answer] a 
	  join zbox.users u on u.userid = a.userid
	  where a.questionid = @CommentId
      and a.boxid =  @BoxId
      and a.AnswerId < @AnswerId
	  order by id desc
	   offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY;";

        public const string GetItemsInReply = @" select
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
      and a.AnswerId < @AnswerId
      and a.boxid =  @BoxId
	  order by answerid desc
	   offset @pageNumber*@rowsperpage ROWS
	            FETCH NEXT @rowsperpage ROWS ONLY);";
    }
}