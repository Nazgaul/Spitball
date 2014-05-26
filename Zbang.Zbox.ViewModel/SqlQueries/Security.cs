namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Security
    {
        public const string GetBoxPrivacySettings = @"select 
b.PrivacySetting
from zbox.Box b 
where b.boxId = @BoxId
and b.IsDeleted = 0;";

        public const string GetUserToBoxRelationShip = @" select top(1) type from (
     select usertype as type from zbox.UserBoxRel 
        where boxid = @BoxId and userid = @UserId
        union
        select 1 as type from zbox.Message
        where TypeOfMsg = 2 and RecepientId = @UserId and BoxId = @BoxId
	    and isActive = 1) t
	    order by 1 desc;";

    }
}
