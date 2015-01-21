namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Search
    {
//        public const string OwnedSubscribedBoxes = @"select  b.pictureurl as image,
// b.BoxName as name,
// b.ProfessorName as professor,
// b.CourseCode as courseCode,
// b.BoxId as id ,
//b.Url as url
//from zbox.box b 
//join zbox.UserBoxRel ub on ub.BoxId = b.BoxId and ub.UserId = @userId
//where b.IsDeleted = 0
// and (b.BoxName like '%' + @query + '%'
//	or b.CourseCode like '%' + @query + '%'
//	or b.ProfessorName like '%' + @query + '%')
//order by len(b.BoxName) - len(REPLACE(b.BoxName,@query,'')) / len(@query) asc, len(b.boxName)
//offset @pageNumber*@rowsperpage rows
//fetch next @rowsperpage rows only; ";

//        public const string UniversityBoxes = @"select b.pictureurl as image,
// b.BoxName as name,
// b.ProfessorName as professor,
// b.CourseCode as courseCode,
// b.BoxId as id ,
//b.Url as url
//from zbox.box b
//where b.IsDeleted = 0
//and b.University = @universityId
//and (b.BoxName like '%' + @query + '%'
//	or b.CourseCode like '%' + @query + '%'
//	or b.ProfessorName like '%' + @query + '%')
//order by len(b.BoxName) - len(REPLACE(b.BoxName,@query,'')) / len(@query) asc, len(b.boxName)
//offset @pageNumber*@rowsperpage rows
//fetch next @rowsperpage rows only;";

        public const string Users = @"select  u.UserImageLarge as image,u.UserName as name, u.UserId as id, u.Url as url
from zbox.users u
where u.UniversityId = @universityId
and u.username like '%' +@query + '%'
order by len(u.username) - len(REPLACE(u.username,@query,'')) / len(@query) asc
offset @pageNumber*@rowsperpage rows
fetch next @rowsperpage rows only;";

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
            @"select top 500 b.boxid  as Id,b.PictureUrl as Image
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
  b.BoxName as boxname,
  i.Content as content,
  i.blobName as blobName,
  i.Rate as rate,
  i.NumberOfViews as views,
  i.Url as url,
  u.UniversityName as universityName,
  u.id as universityid,
  b.BoxId as boxid
   from zbox.item i 
   join zbox.box b on i.BoxId = b.BoxId
   left join zbox.University u on b.University = u.id
  where i.itemid=122 -- i.isdirty =1
   and i.IsDeleted = 0
    order by i.ItemId";

        public const string GetItemUsersToUploadToSearch =
            @"  select UserId,BoxId from zbox.UserBoxRel where boxId in (
select top 500 i.boxid  from zbox.item i  join zbox.box b on i.BoxId = b.BoxId
   left join zbox.University u on b.University = u.id
  where i.itemid=122 --(i.isdirty = 1 ) 
  and i.isdeleted = 0 
  order by i.ItemId)";
    }

    
}
