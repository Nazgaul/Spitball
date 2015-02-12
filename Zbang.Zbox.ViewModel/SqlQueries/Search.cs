﻿namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Search
    {

        public const string Items = @"select 
i.thumbnailurl as image,
i.Name as name,
i.ItemId as id,
i.Content as content,
i.Rate as rate,
i.NumberOfViews as views,
b.BoxName as boxname,
b.BoxId as boxid ,
'' as uniName,
i.url as Url
from zbox.item i
join zbox.box b on i.BoxId = b.BoxId and b.IsDeleted = 0
where i.IsDeleted = 0
and b.University = @universityId
and b.Discriminator = 2
and (i.Name like '%' +@query + '%')
order by len(i.Name) - len(REPLACE(i.name,@query,'')) / len(@query) asc
offset @pageNumber*@rowsperpage rows
fetch next @rowsperpage rows only;";

        public const string ItemFromOtherUniversities = @"
select i.thumbnailurl as image,
i.Name as name,
i.ItemId as id,
i.Content as content,
i.Rate as rate,
i.NumberOfViews as views,
b.BoxName as boxname,
b.BoxId as boxid ,
universityname as uniName,
i.url as Url
from zbox.item i
join zbox.box b on i.BoxId = b.BoxId and b.IsDeleted = 0
join zbox.users u2 on u2.UserId = b.OwnerId
where i.IsDeleted = 0
and b.University in (
select id from 
zbox.University u 
where u.NeedCode = 0 
and u.country = (select country from zbox.University where Id = @universityId)
and u.Id != @universityid
)
and b.Discriminator = 2
and (i.Name like '%' +@query + '%')
order by len(i.Name) - len(REPLACE(i.name,@query,'')) / len(@query) asc, uniName
offset @pageNumber*@rowsperpage rows
fetch next @rowsperpage rows only;";

        public const string GetBoxToUploadToSearch =
            @"select top 500 b.boxid  as Id
,b.BoxName as Name, b.ProfessorName as Professor ,b.CourseCode as CourseCode
, b.Url as Url, b.University as UniversityId , b.PrivacySetting as PrivacySetting
  from zbox.box b
  where isdirty = 1 and isdeleted = 0
  order by b.BoxId;";

        public const string GetBoxUsersToUploadToSearch = @"select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top 500 b.boxid  from zbox.box b
  where isdirty = 1 and isdeleted = 0 
  order by b.BoxId);";


        public const string GetBoxToDeleteToSearch = @"
        select top 500 boxid as id from zbox.box
        where isdirty = 1 and isdeleted = 1;";

        public const string GetItemToDeleteToSearch = @"
        select top 500 itemid as id from zbox.item
        where isdirty = 1 and isdeleted = 1;";

        public const string GetItemsToUploadToSearch =
            @" select top 500
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
   and i.discriminator = 'File'
    order by i.ItemId";

        public const string GetItemUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top 500 i.boxid  from zbox.item i  join zbox.box b on i.BoxId = b.BoxId
   left join zbox.University u on b.University = u.id
  where i.isdirty = 1 
  and i.isdeleted = 0 
  and i.discriminator = 'File'
  order by i.ItemId)";

        public const string GetQuizzesUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
	select top 500 q.BoxId
	from zbox.quiz q 
	where publish = 1
	and q.isdeleted = 0
	and q.isdirty = 1
	order by Id);";

        public const string GetQuizzesToUploadToSearch = @"select top 500 q.Id,
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
            @"select text, QuizId  from zbox.QuizQuestion where QuizId in (
select top 500 q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
order by Id);";


        public const string GetQuizzesAnswersToUploadToSearch =
            @"select text,QuizId from zbox.QuizAnswer where QuizId in (
select top 500 q.Id
from zbox.quiz q 
where publish = 1
and q.isdeleted = 0
and q.isdirty = 1
order by Id);";

        public const string GetQuizzesToDeleteFromSearch = @"
        select top 500 id as id from zbox.Quiz
        where isdirty = 1 and isdeleted = 1;";

    }




}
