﻿namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Search
    {
        public const string GetBoxToUploadToSearch =
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
  from zbox.box b
  where isdirty = 1 and isdeleted = 0 and url is not null and b.boxid % @count  = @index
  order by b.BoxId;";

        public const string GetBoxUsersToUploadToSearch = @"select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top (@top) b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null and b.boxid % @count  = @index
  order by b.BoxId);";

        public const string GetBoxDepartmentToUploadToSearch = @"  with c as (
select l.*, b.boxid from zbox.library l join zbox.box b on l.libraryid = b.libraryid and b.boxid in 
( select top (@top) b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null and b.boxid % @count  = @index
  order by b.BoxId)
 
	union all
	select t.*, c.boxid from zbox.library t inner join c on c.parentid = t.libraryid
)

select Name,boxid from c";


        public const string GetBoxToDeleteToSearch = @"
        select top 500 boxid as id from zbox.box
        where isdirty = 1 and isdeleted = 1 and boxid % @count  = @index;";

        public const string GetItemToDeleteToSearch = @"
        select top 10 itemid as id from zbox.item
        where isdirty = 1 and isdeleted = 1 and itemid % @count  = @index;";

        public const string GetItemsToUploadToSearch =
            @" select top (@top)
  i.ItemId as id,
  i.Name as name,
  i.Content as content,
  i.blobName as blobName,
  i.Url as url,
  i.discriminator as type,
  case b.Discriminator
   when 2 then
       b.University
	   else null
	   end
   as universityid,
  b.BoxName as boxname,
  u.UniversityName as universityName,
  b.BoxId as boxid
    from zbox.item i 
    join zbox.box b on i.BoxId = b.BoxId
    left join zbox.University u on b.University = u.id
    where i.isdirty = 1 
    and i.IsDeleted = 0
    and b.isdeleted = 0 -- performance
    and i.itemid % @count  = @index
    order by i.ItemId desc";

        public const string GetItemToUploadToSearch =
            @"select
  i.ItemId as id,
  i.Name as name,
  i.Content as content,
  i.blobName as blobName,
  i.Url as url,
  i.discriminator as type,
  case b.Discriminator
   when 2 then
       b.University
	   else null
	   end
   as universityid,
  b.BoxName as boxname,
  u.UniversityName as universityName,
  b.BoxId as boxid
    from zbox.item i 
    join zbox.box b on i.BoxId = b.BoxId
    left join zbox.University u on b.University = u.id
    where i.itemid = @itemId";

        public const string GetItemUsersToUploadToSearch = @"
 select UserId from zbox.UserBoxRel where boxId in (
select boxid  from zbox.item i  
  where   i.itemid = @itemId
 )";

        public const string GetItemsUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top (@top) i.boxid  from zbox.item i  
  where  i.isdirty = 1 
  and i.isdeleted = 0 
   and i.itemid % @count  = @index
  order by i.ItemId desc)";

        public const string GetQuizzesUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
	select top 100 q.BoxId
	from zbox.quiz q 
	where publish = 1
	and q.isdeleted = 0
	and q.isdirty = 1
    and q.id % @count  = @index
	order by Id);";

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


        public const string GetUsersInBoxByTerm = @"select u.userid as Id, username as Name,UserImageLarge as Image 
from zbox.users u 
where username like  @Term + '%'
and u.userid not in ( select userid from zbox.UserBoxRel where boxid = @BoxId)
order by
case when u.UniversityId = @UniversityId then 0 else 1 end asc, UserReputation desc
offset @pageNumber*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY; ";

        public const string GetUsersByTerm =
            @"select userid as id,username as name, UserImageLarge as image, online, Url as url from zbox.users u
where u.UserName like  @term + '%'
and u.userid <> @UserId
order by 
case when u.UniversityId = @UniversityId then 0 else 1 end asc, userid
offset @pageNumber*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY;";






        public const string GetUniversityToUploadToSearch = @"select top (@top) id as Id,UniversityName as Name,LargeImage as Image,
extra as Extra, Country, NoOfUsers
from zbox.University
where isdirty = 1 and isdeleted = 0 
and id % @count  = @index;";


        public const string GetUniversityPeopleToUploadToSearch = @"select universityId, UserImageLarge as Image from (
select userid, universityid, UserImageLarge, rowid = ROW_NUMBER() over (partition by UniversityId order by UserImageLarge desc) from zbox.Users u where UniversityId in (
select top (@top) id
from zbox.University u
where isdirty = 1 and isdeleted = 0 )) t
where t.rowid < 6";

        public const string GetUniversitiesToDeleteFromSearch = @"select top 500 id from zbox.University
                    where isdirty = 1 and isdeleted = 1 and id % @count  = @index;";
    }




}
