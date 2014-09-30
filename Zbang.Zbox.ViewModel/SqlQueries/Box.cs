
namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Box
    {
        public const string BoxData = @"
select 
  b.PictureUrl as Image
 ,b.BoxName as Name
 ,b.CourseCode as CourseId
 ,b.ProfessorName as ProfessorName
 ,b.ItemCount as Items
 ,b.MembersCount as Members
 ,b.PrivacySetting
 ,b.quizcount as Quizes
 ,b.OwnerId as OwnerId
 ,b.Discriminator as BoxType
 ,b.CreationTime as Date
 ,(CASE 
      WHEN b.Discriminator is null THEN (select username from zbox.Users where userid = b.ownerid)
      Else (select universityname from zbox.university where id = b.university) 
   END ) as OwnerName
from zbox.box b
where b.BoxId = @BoxId
and b.IsDeleted = 0;
";
        public const string BoxTabs = @"
select 
ItemTabId as id
,itemtabname as name
 from zbox.ItemTab
where boxid = @BoxId";
    }
}
