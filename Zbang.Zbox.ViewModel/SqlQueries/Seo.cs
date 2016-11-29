namespace Zbang.Zbox.ViewModel.SqlQueries
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Seo")]
    public static class Seo
    {


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Seo")]
        public const string GetSeoItemsByPage = @"with boxSeo as (
 select BoxId, url from zbox.box b 
where University in (select v.id from zbox.University v where needcode = 0)
and Discriminator = 2
and IsDeleted = 0
)
select b.boxid, b.Url +'feed/' as url 
from boxseo b
union all
select b.boxid, b.Url +'items/' as url 
from boxseo b
union all
select b.boxid, b.Url +'quizzes/' as url 
from boxseo b
union all
select b.boxid,i.url as url
from zbox.item i join boxSeo b on i.BoxId = b.BoxId
where i.IsDeleted = 0
and i.content is not null
and i.Discriminator = 'FILE'
union all
select b.boxid,q.url as Url
from zbox.quiz q join boxSeo b on q.BoxId = b.BoxId
where q.IsDeleted = 0
and q.publish = 1
order by 1
offset (@pageNumber-1)*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY;";

        public const string GetSeoItemsCount = @"select sum(c)  from (
select count(*)*3 as c
from zbox.box b 
where University in (select id from zbox.University where  needcode = 0)
and Discriminator = 2
and IsDeleted = 0
union all
select count(*)
from zbox.item i 
join zbox.box b on b.BoxId = i.BoxId
where b.University   in (select id from zbox.University where  needcode = 0)
and b.discriminator = 2
and i.IsDeleted = 0
and i.content is not null
and i.Discriminator = 'FILE'
union all
select count(*)
from zbox.quiz i 
join zbox.box b on b.BoxId = i.BoxId
where b.University in (select id from zbox.University where  needcode = 0)
and i.IsDeleted = 0
and b.discriminator = 2
and i.publish = 1 ) t";


        public const string FileSeo = @"select i.name
,i.Content as Description
,i.Url
,u.Country
,b.BoxName
,i.Discriminator
,i.blobname as ImageUrl
,l.name as departmentName
from zbox.item i 
join zbox.box b on i.BoxId = b.BoxId
left join zbox.University u on b.University = u.Id
left join zbox.library l on b.libraryid = l.libraryid
where itemid = @ItemId
and i.IsDeleted = 0;";

        public const string FlashcardSeo = @"select u.universityname as UniversityName, b.boxid as BoxId,b.boxname as BoxName,f.id,f.name 
from zbox.flashcard f
join zbox.box b on f.boxid = b.boxid
left join zbox.university u on u.id = b.university
where f.id = @FlashcardId
and f.isdeleted = 0";

        public const string BoxSeo = @"select b.BoxName as name 
,b.url
,b.Discriminator as boxType
,u.Country
,u.UniversityName
,l.name as DepartmentName

from zbox.box b
left join zbox.University u on b.University = u.id
left join zbox.library l on b.libraryid = l.libraryid
where boxid = @BoxId
and b.IsDeleted = 0";
    }
}
