namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Seo
    {
        public const string GetBoxes = @"select b.BoxId as Id,b.BoxName as Name,u.UniversityName as UniversityName
from zbox.box b join zbox.Users u on b.OwnerId = u.UserId
where ownerid in (select userid from zbox.users where usertype =1 and needcode = 0)
and Discriminator = 2
and IsDeleted = 0;";

        public const string GetItems = @"select b.BoxId as BoxId,b.BoxName as BoxName,i.ItemId as Id ,i.Name as Name,u.UniversityName as UniversityName
from zbox.item i 
join zbox.box b on b.BoxId = i.BoxId
join zbox.Users u on b.OwnerId = u.UserId
where b.OwnerId   in (select userid from zbox.users where usertype =1 and needcode = 0)
and b.IsDeleted = 0
and b.discriminator = 2
and i.IsDeleted = 0;";


    }
}
