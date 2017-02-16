namespace Zbang.Zbox.ViewModel.SqlQueries
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


        public const string GetItemsToUploadToSearch2 = @"
select 
  i.ItemId as Id,
  i.Name as name,
  i.blobName as blobName,
  i.Url as url, --old
  i.discriminator as typeDocument,
  b.University as universityid, -- old
  b.ProfessorName as BoxProfessor,
  b.CourseCode as BoxCode,
  b.BoxName as boxname,
  u.UniversityName as universityName,
  b.BoxId as boxid,
  it.ItemTabName as TabName,
  i.CreationTime as Date,
  ub.UserId as UserIds_Id,
  i.language,
  t.Name as Tags_Name,
  itag.Type as Tags_Type
    from zbox.item i 
    join zbox.box b on i.BoxId = b.BoxId
	left join zbox.UserBoxRel ub on b.BoxId = ub.BoxId
    left join zbox.University u on b.University = u.id
	left join zbox.ItemTab it on i.ItemTabId = it.ItemTabId
	left join zbox.ItemTag itag on itag.ItemId = i.ItemId left join zbox.Tag t on itag.TagId = t.Id
	where (@top is null or (i.ItemId in (
	select top (@top) itemid from zbox.Item 
    where isdirty = 1 
    and IsDeleted = 0
    and creationtime < DATEADD(minute, -1, getutcdate())
    and itemid % @count  = @index
    order by ItemId desc)))
	and (@itemId is null or (i.ItemId = @itemId));
";


        //       public const string GetItemToUploadToSearch =
        //           @"select 
        // i.ItemId as id,
        // i.Name as name,
        // i.blobName as blobName,
        // i.Url as url,
        // i.discriminator as type,
        // b.University as universityid,
        // b.ProfessorName as BoxProfessor,
        // b.CourseCode as BoxCode,
        // b.BoxName as boxname,
        // u.UniversityName as universityName,
        // b.BoxId as boxid,
        // it.ItemTabName,
        // i.CreationTime as Date,
        // ub.UserId as UserIds_Id,
        // i.language,
        // t.Name as Tags_Name
        //   from zbox.item i 
        //   join zbox.box b on i.BoxId = b.BoxId
        //left join zbox.UserBoxRel ub on b.BoxId = ub.BoxId
        //   left join zbox.University u on b.University = u.id
        //left join zbox.ItemTab it on i.ItemTabId = it.ItemTabId
        //left join zbox.ItemTag itag on itag.ItemId = i.ItemId join zbox.Tag t on itag.TagId = t.Id
        //where i.ItemId = @itemId;";



        public const string GetQuizzesUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
	select top 100 q.BoxId
	from zbox.quiz q 
	where publish = 1
	and q.isdeleted = 0
	and q.isdirty = 1
    and q.id % @count  = @index
	order by Id);";



        public const string GetFlashcardToUploadToSearch = @"select f.Id,
f.Name,
 b.BoxName,
 f.BoxId,
 f.language,
 ub.UserId as UserIds_Id,
 u.UniversityName as universityName,
       b.University as UniversityId,
 t.Name as Tags_Name,
 itag.Type as Tags_Type
    from zbox.Flashcard f
join zbox.Box b on f.BoxId = b.BoxId
left join zbox.University u on b.University = u.id
left join zbox.UserBoxRel ub on b.BoxId = ub.BoxId
left join zbox.ItemTab it on i.ItemTabId = it.ItemTabId
left join zbox.ItemTag itag on itag.ItemId = i.ItemId join zbox.Tag t on itag.TagId = t.Id
where f.id in (select top (@top) Id from zbox.Flashcard x where x.publish = 1
and x.isdeleted = 0
and x.isdirty = 1
and x.id % @count  = @index
order by x.Id desc);";

        public const string GetQuizzesToUploadToSearch = @"select top (@top) q.Id,
q.Name,
 b.BoxName,
 q.BoxId,
 q.Url,
 u.UniversityName as universityName,
  case b.Discriminator
   when 2 then
       b.University
	   else null
	   end
   as universityid
from zbox.quiz q 
join zbox.Box b on q.BoxId = b.BoxId
left join zbox.University u on b.University = u.id
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
and q.id % @count  = @index
order by Id;";


        public const string GetQuizzesQuestionToUploadToSearch =
            @"select text, QuizId, Id as questionid  from zbox.QuizQuestion where QuizId in (
select top (@top) q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
and q.id % @count  = @index
order by Id)
order by Id;";


        public const string GetQuizzesAnswersToUploadToSearch =
            @"select text,QuizId, questionid from zbox.QuizAnswer where QuizId in (
select top (@top) q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
and q.id % @count  = @index
order by Id)
order by QuestionId,Id;";

        public const string GetQuizzesToDeleteFromSearch = @"
        select top (@top) id as id from zbox.Quiz
        where isdirty = 1 and isdeleted = 1 and id % @count  = @index;";


        public const string GetFlashcardToDeleteFromSearch = @" select top (@top) id as id from zbox.flashcard
        where isdirty = 1 and isdeleted = 1 and id % @count  = @index;";


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


        public const string GetUniversitiesToUploadToSearch = @"select top (@top) id as Id,UniversityName as Name,LargeImage as Image,
extra as Extra, Country, NoOfUsers
from zbox.University
where isdirty = 1 and isdeleted = 0 
and id % @count  = @index;";

        public const string GetUniversityToUploadToSearch =
            @"select id as Id,UniversityName as Name,LargeImage as Image,
extra as Extra, Country, NoOfUsers
from zbox.University
where id = @id";


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
where t.rowid < 6";

        public const string GetUniversitiesToDeleteFromSearch = @"select top 500 id from zbox.University
                    where isdirty = 1 and isdeleted = 1 and id % @count  = @index;";




        #region Document

        public const string SearchItemNew = @"select top (@top)
  i.ItemId as id,
  i.Name as name,
  i.Content as documentcontent,
  i.blobName as blobName,
  i.Url as url, --old
  i.discriminator as typeDocument,
  b.ProfessorName as BoxProfessor,
  b.CourseCode as BoxCode,
  case b.Discriminator
   when 2 then
       b.University
	   else null
	   end
   as universityid,
  b.BoxName as boxname, -- old
  u.UniversityName as universityName,
  b.BoxId as boxid,
  i.CreationTime as Date,
  i.language,
  i.LikeCount as likes,
  i.NumberOfViews as views,
  it.ItemTabName as TabName
    from zbox.item i 
    join zbox.box b on i.BoxId = b.BoxId
    left join zbox.University u on b.University = u.id
	left join zbox.ItemTab it on i.ItemTabId = it.ItemTabId
    where i.isdirty = 1 
    and i.IsDeleted = 0
    and i.creationtime < DATEADD(minute, -1, getutcdate())
    and b.isdeleted = 0 -- performance
    and i.itemid % @count  = @index
	and (@itemId is null or (i.ItemId = @itemId))
    order by i.ItemId desc;";

        public const string SearchItemUserBoxRel = @"select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top (@top) i.boxid  from zbox.item i  
  where  i.isdirty = 1 
  and i.isdeleted = 0 
  and i.creationtime < DATEADD(minute, -1, getutcdate())
   and i.itemid % @count  = @index
   and (@itemId is null or (i.ItemId = @itemId))
  order by i.ItemId desc);";

        public const string SearchItemTags =
            @"select it.ItemId, t.Name, it.Type from zbox.ItemTag it join zbox.Tag t on it.TagId = t.Id where it.ItemId in (
  select top (@top) i.ItemId  from zbox.item i  
  where  i.isdirty = 1 
  and i.isdeleted = 0 
  and i.creationtime < DATEADD(minute, -1, getutcdate())
   and i.itemid % @count  = @index
   and (@itemId is null or (i.ItemId = @itemId))
  order by i.ItemId desc);";

        #endregion
    }
}