namespace Zbang.Zbox.ViewModel.SqlQueries
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Seo")]
    public static class Seo
    {


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Seo")]
        public const string GetSeoItemsByPage = @"select b.Url as url
from zbox.box b 
where University in (select v.id from zbox.University v where needcode = 0)
and Discriminator = 2
and IsDeleted = 0
union all
select i.url as url
from zbox.item i 
join zbox.box b on b.BoxId = i.BoxId
where b.University   in (select v.id from zbox.University v where needcode = 0)
and b.discriminator = 2
and i.IsDeleted = 0
and i.Discriminator = 'FILE'
union all
select i.url as Url
from zbox.quiz i 
join zbox.box b on b.BoxId = i.BoxId
where b.University in (select id from zbox.University where needcode = 0)
and i.IsDeleted = 0
and b.discriminator = 2
and i.publish = 1
order by 1
offset (@pageNumber-1)*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY";

        public const string GetSeoItemsCount = @"
select count(*) from (
select b.Url as url
from zbox.box b 
where University in (select id from zbox.University where  needcode = 0)
and Discriminator = 2
and IsDeleted = 0
union all
select i.url as url
from zbox.item i 
join zbox.box b on b.BoxId = i.BoxId
where b.University   in (select id from zbox.University where  needcode = 0)
and b.discriminator = 2
and i.IsDeleted = 0
and i.Discriminator = 'FILE'
union all
select i.url as Url
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

        public const string BoxSeo = @"select b.BoxName as name 
,b.CourseCode as courseId
,b.ProfessorName as professor
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
