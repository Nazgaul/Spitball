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

        public const string GetInviteToBoxInvite = @" 
        select 1 from zbox.Invite where id=@InviteId and IsUsed = 0
      ";

        public const string GetUserAccessParams = @"with boxdata as (
 select boxid, Discriminator, PrivacySetting, libraryparentid from zbox.Box where boxid = @boxid and IsDeleted = 0
)
select bd.PrivacySetting, ub.usertype as UserType, Discriminator as BoxType,
(select usertype from zbox.userlibraryrel where userid = @userid and LibraryId = bd.libraryparentid) as LibraryUserType
from boxdata bd 
left join zbox.UserBoxRel ub on bd.BoxId = ub.BoxId and ub.UserId = @userid;";
    }
}
