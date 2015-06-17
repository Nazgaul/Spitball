﻿namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Search
    {
        public const string GetBoxToUploadToSearch =
            @"select top 500 b.boxid  as Id
,b.BoxName as Name, b.ProfessorName as Professor ,b.CourseCode as CourseCode
, b.Url as Url, b.University as UniversityId ,  b.discriminator as Type
  from zbox.box b
  where isdirty = 1 and isdeleted = 0 and url is not null
  order by b.BoxId;";

        public const string GetBoxUsersToUploadToSearch = @"select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top 500 b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null
  order by b.BoxId);";

        public const string GetBoxDepartmentToUploadToSearch = @"  with c as (
select l.*, b.boxid from zbox.library l join zbox.box b on l.libraryid = b.libraryid and b.boxid in ( select top 500 b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0  and url is not null
  order by b.BoxId)
 
	union all
	select t.*, c.boxid from zbox.library t inner join c on c.parentid = t.libraryid
)

select Name,boxid from c";


        public const string GetBoxToDeleteToSearch = @"
        select top 500 boxid as id from zbox.box
        where isdirty = 1 and isdeleted = 1;";

        public const string GetItemToDeleteToSearch = @"
        select top 10 itemid as id from zbox.item
        where isdirty = 1 and isdeleted = 1;";

        public const string GetItemsToUploadToSearch =
            @" select top 1
  i.ItemId as id,
  i.Name as name,
  i.ThumbnailUrl as image,
  i.Content as content,
  i.blobName as blobName,
  i.Url as url,
  u.id as universityid,
  b.BoxName as boxname,
  u.UniversityName as universityName,
  b.BoxId as boxid
   from zbox.item i 
   join zbox.box b on i.BoxId = b.BoxId
   left join zbox.University u on b.University = u.id
   where i.isdirty = 1 
   and i.IsDeleted = 0
    and i.itemid % @count  = @index
   and i.discriminator = 'File'
    order by i.ItemId desc";

        public const string GetItemUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top 1 i.boxid  from zbox.item i  join zbox.box b on i.BoxId = b.BoxId
   left join zbox.University u on b.University = u.id
  where  i.isdirty = 1 
  and i.isdeleted = 0 
  and i.discriminator = 'File'
   and i.itemid % @count  = @index
  order by i.ItemId desc)";

        public const string GetQuizzesUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
	select top 100 q.BoxId
	from zbox.quiz q 
	where publish = 1
	and q.isdeleted = 0
	and q.isdirty = 1
	order by Id);";

        public const string GetQuizzesToUploadToSearch = @"select top 100 q.Id,
q.Name,
 b.BoxName,
 q.BoxId,
 q.Url,
 u.UniversityName as universityName,
 u.id as universityid
from zbox.quiz q 
join zbox.Box b on q.BoxId = b.BoxId
left join zbox.University u on b.University = u.id
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
order by Id;";


        public const string GetQuizzesQuestionToUploadToSearch =
            @"select text, QuizId, Id as questionid  from zbox.QuizQuestion where QuizId in (
select top 100 q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
order by Id)
order by Id;";


        public const string GetQuizzesAnswersToUploadToSearch =
            @"select text,QuizId, questionid from zbox.QuizAnswer where QuizId in (
select top 100 q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
order by Id)
order by QuestionId,Id;";

        public const string GetQuizzesToDeleteFromSearch = @"
        select top 100 id as id from zbox.Quiz
        where isdirty = 1 and isdeleted = 1;";

    }




}
