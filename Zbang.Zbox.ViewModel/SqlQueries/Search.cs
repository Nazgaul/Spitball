﻿namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Search
    {
        public const string GetBoxesToUploadToSearch =
            @"select top (@top) 
b.boxid  as Id
,b.BoxName as Name
, b.ProfessorName as Professor
,b.CourseCode as CourseCode
, b.Url as Url
, case b.Discriminator
   when 2 then
       b.University
	   else null
	   end
   as universityid
,  b.discriminator as Type
, b.LibraryId as DepartmentId
, b.MembersCount as MembersCount
, b.ItemCount + b.FlashcardCount + b.QuizCount as ItemsCount
  from zbox.box b
  where isdirty = 1 and isdeleted = 0 and url is not null and b.boxid % @count  = @index
  order by b.BoxId;";

        public const string GetBoxToUploadToSearch =
            @"select 
b.boxid  as Id
,b.BoxName as Name
, b.ProfessorName as Professor
,b.CourseCode as CourseCode
, b.Url as Url
, case b.Discriminator
   when 2 then
       b.University
	   else null
	   end
   as universityid
,  b.discriminator as Type
, b.LibraryId as DepartmentId
, b.MembersCount as MembersCount
, b.ItemCount + b.FlashcardCount + b.QuizCount as ItemsCount
  from zbox.box b
  where boxid = @Boxid ;";

        public const string GetBoxUsersToUploadToSearch = @"select UserId from zbox.UserBoxRel where boxId =@boxid;";

        public const string GetBoxesUsersToUploadToSearch = @"select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top (@top) b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null and b.boxid % @count  = @index
  order by b.BoxId);";

        public const string GetBoxFeedToUploadToSearch = @"select text,boxid from zbox.question
  where boxid =@boxid
  and IsSystemGenerated = 0
  and text is not null
  union all
  select text,boxid from zbox.answer
  where boxid =@boxid
  and text is not null;";
        public const string GetBoxesFeedToUploadToSearch = @"select text,boxid from zbox.question
  where boxid in (select top (@top) b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null and b.boxid % @count  = @index
  order by b.BoxId)
  and IsSystemGenerated = 0
  and text is not null
  union all
  select text,boxid from zbox.answer
  where boxid in (select top (@top) b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null and b.boxid % @count  = @index
  order by b.BoxId)
  and text is not null;";
        public const string GetBoxDepartmentToUploadToSearch = @"
with c as (
select l.* from zbox.library l join zbox.box b on l.libraryid = b.libraryid and b.boxid =@boxid
 
	union all
	select t.* from zbox.library t inner join c on c.parentid = t.libraryid
)

select Name from c;";
        public const string GetBoxesDepartmentToUploadToSearch = @"  with c as (
select l.*, b.boxid from zbox.library l join zbox.box b on l.libraryid = b.libraryid and b.boxid in 
( select top (@top) b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null and b.boxid % @count  = @index
  order by b.BoxId)
 
	union all
	select t.*, c.boxid from zbox.library t inner join c on c.parentid = t.libraryid
)

select Name,boxid from c;";


        public const string GetBoxesToDeleteToSearch = @"
        select top 500 boxid as id from zbox.box
        where isdirty = 1 and isdeleted = 1 and boxid % @count  = @index;";

        public const string GetItemToDeleteToSearch = @"
        select top 10 itemid as id from zbox.item
        where isdirty = 1 and isdeleted = 1 and itemid % @count  = @index;";

        #region Quiz
          public const string GetQuizzesToUploadToSearch = @"select top (@top) 
q.Id,
q.Name as QuizName,
 q.Language,
 q.CreationTime as Date,
 q.Url,
 q.LikeCount as Likes,
 q.NumberOfViews as Views,
 q.BoxId as id,
b.BoxName as name,
 b.ProfessorName as Professor,
  b.CourseCode as Code,
   b.University as id ,
 u.UniversityName as name
from zbox.quiz q 
join zbox.Box b on q.BoxId = b.BoxId
left join zbox.University u on b.University = u.id
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
and q.id % @count  = @index
order by q.Id desc;";


        public const string GetQuizzesQuestionToUploadToSearch =
            @"select text, QuizId, Id as questionId  from zbox.QuizQuestion where QuizId in (
select top (@top) q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
and q.id % @count  = @index
order by Id desc)
order by Id;";

        public const string GetQuizzesAnswersToUploadToSearch =
          @"select text,QuizId, questionid from zbox.QuizAnswer where QuizId in (
select top (@top) q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
and q.id % @count  = @index
order by Id  desc)
order by QuestionId,Id;";

        public const string GetQuizzesUsersToUploadToSearch =
          @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
	select top 100 q.BoxId
	from zbox.quiz q 
	where publish = 1
	and q.isdeleted = 0
	and q.isdirty = 1
    and q.id % @count  = @index
	order by Id  desc);";

        public const string GetQuizzesTags = @"select it.QuizId as Id, t.Name, it.Type 
from zbox.ItemTag it 
join zbox.Tag t on it.TagId = t.Id 
where it.QuizId in (
  select top (@top) f.Id  from zbox.quiz f
   where f.publish = 1
        and f.isdeleted = 0
        and f.isdirty = 1
           and f.id % @count  = @index
        order by Id desc);";

        public const string GetQuizzesToDeleteFromSearch = @"
        select top (@top) id as id from zbox.Quiz
        where isdirty = 1 and isdeleted = 1 and id % @count  = @index;";
        #endregion










      


        public const string GetUsersInBox = @"
select u.userid as Id, username as Name,UserImageLarge as Image 
from zbox.users u 
where u.userid not in ( select userid from zbox.UserBoxRel where boxid = @BoxId)
order by
case when u.UniversityId = @UniversityId then 0 else 1 end asc, Score desc
offset @pageNumber*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY;";

        public const string GetUsersInBoxByTerm =

            @"select u.userid as Id, username as Name,UserImageLarge as Image 
from zbox.users u 
where username like  @Term + '%'
and u.userid not in ( select userid from zbox.UserBoxRel where boxid = @BoxId)
order by
case when u.UniversityId = @UniversityId then 0 else 1 end asc, Score desc
offset @pageNumber*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY; ";


        public const string GetUniversitiesToUploadToSearch = @"
select top (@top) id as Id,UniversityName as Name,LargeImage as Image,
extra as Extra, Country, NoOfUsers, Latitude as Latitude, Longitude as Longitude
from zbox.University
where isdirty = 1 and isdeleted = 0 
and id % @count  = @index;";

        public const string GetUniversityToUploadToSearch =
            @"select id as Id,UniversityName as Name,LargeImage as Image,
extra as Extra, Country, NoOfUsers, Latitude as Latitude, Longitude as Longitude
from zbox.University
where id = @id;";


        public const string GetUniversitiesPeopleToUploadToSearch = @"select universityId, UserImageLarge as Image from (
select userid, universityid, UserImageLarge, rowid = ROW_NUMBER() over (partition by UniversityId order by UserImageLarge desc) from zbox.Users u where UniversityId in (
select top (@top) id
from zbox.University u
where isdirty = 1 and isdeleted = 0 )) t
where t.rowid < 6";

        public const string GetUniversityPeopleToUploadToSearch = @"select universityId, UserImageLarge as Image from (
select userid, universityid, UserImageLarge, rowid = ROW_NUMBER() over (partition by UniversityId order by UserImageLarge desc)
from zbox.Users u where UniversityId = @id
) t
where t.rowid < 6;";

        public const string GetUniversitiesToDeleteFromSearch = @"select top 500 id from zbox.University
                    where isdirty = 1 and isdeleted = 1 and id % @count  = @index;";



        #region Flashcard
        public const string GetFlashcardToUploadToSearch = @"select top (@top) 
f.Id,
f.Name as FlashcardName,
f.language,

 f.CreationTime as Date,
	   f.CardCount as CardsCount,
	   f.likecount as likes,
	   f.numberofviews as views,
  b.BoxId as id,
  b.ProfessorName as Professor,
  b.CourseCode as Code,
  b.BoxName as name, 
  b.University as Id,
  u.UniversityName as Name
    from zbox.Flashcard f
join zbox.Box b on f.BoxId = b.BoxId
left join zbox.University u on b.University = u.id
where publish = 1
and f.isdeleted = 0
and f.isdirty = 1
and f.id % @count  = @index
order by f.Id desc;";

        public const string GetFlashcardUsersToUploadToSearch =
           @" select UserId,BoxId from zbox.UserBoxRel where boxId in (
        select top (@top) f.BoxId
        from zbox.Flashcard f 
        where publish = 1
        and f.isdeleted = 0
        and f.isdirty = 1
           and f.id % @count  = @index
        order by Id desc);";

        public const string GetFlashcardTagsToUploadToSearch =
            @"select it.FlashcardId as Id, t.Name, it.Type 
from zbox.ItemTag it 
join zbox.Tag t on it.TagId = t.Id 
where it.FlashcardId in (
  select top (@top) f.Id  from zbox.Flashcard f
   where publish = 1
        and f.isdeleted = 0
        and f.isdirty = 1
           and f.id % @count  = @index
        order by Id desc);";

        public const string GetFlashcardToDeleteFromSearch = @" select top (@top) id as id from zbox.flashcard
        where isdirty = 1 and isdeleted = 1 and id % @count  = @index;";
        #endregion

        #region Feed

        public const string GetFeedToDeleteFromSearch = @"IF (@version > CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.question')))
select questionid, ct.sys_change_version as version from  CHANGETABLE(CHANGES zbox.question, @version) AS CT
where sys_change_operation = 'D';
else 
select top 0 questionid, ct.sys_change_version as version from  CHANGETABLE(CHANGES zbox.question, @version) AS CT
";

        public const string GetFeedAnswers = @"select questionid, text from zbox.answer where questionid in @questionids;";

        public const string GetFeedTags =
            @"select it.CommentId as Id, t.Name, it.Type from zbox.commentTag it join zbox.Tag t on it.TagId = t.Id where it.CommentId in @questionids;";
        public const string GetFeedToSearch = @"
IF (@version >= CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.question')))
SELECT
  p.questionid as id,
 text,
 p.CreationTime as date,
 p.LikeCount,
 p.ReplyCount,
 (select count(*) from zbox.item i where i.isdeleted = 0 and i.questionid = p.questionid and i.boxid = b.boxid) as ItemCount,
 uu.username as userName,
 uu.UserImageLarge as userImage,
 ct.sys_change_version as version,
 b.boxid as id,
 b.boxname as  name,
 b.ProfessorName as professor,
 b.coursecode as code,
 u.id as id,
 u.UniversityName as name
FROM
    zbox.question AS P
RIGHT OUTER JOIN CHANGETABLE(CHANGES zbox.question, @version) AS CT ON  p.questionid = CT.questionid
join zbox.box b on p.boxid = b.boxid
join zbox.university u on b.university = u.id
join zbox.users uu on p.userid = uu.userid
where sys_change_operation in ('I','U')
and isSystemgenerated = 0
and text is not null
and u.id = 173408
order by p.QuestionId
OFFSET @PageSize * (@PageNumber) ROWS
  FETCH NEXT @PageSize ROWS ONLY;;
else 
SELECT
 p.questionid as id,
 text,
 p.CreationTime as date,
 p.LikeCount,
 p.ReplyCount,
 (select count(*) from zbox.item i where i.isdeleted = 0 and i.questionid = p.questionid and i.boxid = b.boxid) as ItemCount,
 uu.username as userName,
 uu.UserImageLarge as userImage,
 CHANGE_TRACKING_MIN_VALID_VERSION(  
                                   OBJECT_ID('zbox.question')) as version,
 b.boxid as id,
 b.boxname as  name,
 b.ProfessorName as professor,
 b.coursecode as code,
 u.id as id,
 u.UniversityName as name
FROM
    zbox.question AS P
join zbox.box b on p.boxid = b.boxid
join zbox.university u on b.university = u.id
join zbox.users uu on p.userid = uu.userid
and isSystemgenerated = 0
and text is not null
and u.id = 173408
order by p.QuestionId
OFFSET @PageSize * (@PageNumber) ROWS
  FETCH NEXT @PageSize ROWS ONLY;
";

        #endregion
        public const string NextVersionChanges = @"select CHANGE_TRACKING_CURRENT_VERSION();";

        #region Document

        public const string SearchItemNew = @"select top (@top)
  i.ItemId as id,
  i.Name as fileName,
  i.Content as documentcontent,
  i.blobName as blobName,
  i.Url as url, --old
  i.discriminator as typeDocument,
  i.CreationTime as Date,
  i.language,
  i.LikeCount as likes,
  i.NumberOfViews as views,
  i.doctype as docType,
  b.BoxId as id,
  b.ProfessorName as Professor,
  b.CourseCode as Code,
  b.BoxName as name, 
  b.University as id,
  u.UniversityName as name,
  i.previewFailed
    from zbox.item i 
    join zbox.box b on i.BoxId = b.BoxId
    left join zbox.University u on b.University = u.id
    where i.isdirty = 0 
    and i.IsDeleted = 0
    and (@itemId is not null or (i.creationtime < DATEADD(minute, -1, getutcdate())))
    and (@itemId is not null or (i.itemid % @count  = @index))
	and (@itemId is null or (i.ItemId = @itemId))
    order by i.ItemId desc;";

        public const string SearchItemUserBoxRel = @"select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top (@top) i.boxid  from zbox.item i  
  where  i.isdirty = 1 
  and i.isdeleted = 0 
  and (@itemId is not null or (i.creationtime < DATEADD(minute, -1, getutcdate())))
    and (@itemId is not null or (i.itemid % @count  = @index))
	and (@itemId is null or (i.ItemId = @itemId))
  order by i.ItemId desc);";

        public const string SearchItemTags =
            @"select it.ItemId as Id, t.Name, it.Type from zbox.ItemTag it join zbox.Tag t on it.TagId = t.Id where it.ItemId in (
  select top (@top) i.ItemId  from zbox.item i  
  where  i.isdirty = 1 
  and i.isdeleted = 0 
 and (@itemId is not null or (i.creationtime < DATEADD(minute, -1, getutcdate())))
    and (@itemId is not null or (i.itemid % @count  = @index))
	and (@itemId is null or (i.ItemId = @itemId))
  order by i.ItemId desc);";

        #endregion
    }
}