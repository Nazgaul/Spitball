
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
 ,0 as Quizes
 ,b.OwnerId as OwnerId
 ,b.Discriminator as BoxType
 ,b.CreationTime as Date
 ,(CASE 
      WHEN b.Discriminator is null THEN (select username from zbox.Users where userid = b.ownerid)
      Else null
   END ) as OwnerName
from zbox.box b
where b.BoxId = @BoxId
and b.IsDeleted = 0;
";
    }
}
