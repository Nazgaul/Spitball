namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Security
    {
        public const string GetBoxPrivacySettings = @"select 
b.PrivacySetting
from zbox.Box b 
where b.boxId = @BoxId
and b.IsDeleted = 0;";

        public const string GetUserToBoxRelationship = @" 
     select usertype as type from zbox.UserBoxRel 
        where boxid = @BoxId and userid = @UserId
	    ;";

    }
}
