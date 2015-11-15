

namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class User
   {
       public const string UserProfileWithStats = @"
		select u.userid as Id, u.username as name, u.UserImageLarge as image,
                            u.userReputation as score, uu.universityname as universityName, u.url as Url
                            from zbox.users u left join zbox.university uu on u.UniversityId = uu.id
                            where u.userid =@Myfriend;
    select count(*)
        from 
    zbox.UserBoxRel uFriend
    join zbox.box b on b.BoxId = uFriend.BoxId and b.IsDeleted = 0
    left join zbox.UserBoxRel uMe on b.BoxId = uMe.BoxId and uMe.UserId = @Me
    where uFriend.UserId = @Myfriend
    and (b.PrivacySetting = 3 or uMe.UserId = @Me)
	union all
		 select count(*)
                        from zbox.item i 
                        join zbox.box b on i.boxid = b.BoxId and b.IsDeleted = 0
                        left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where i.UserId = @Myfriend
                        and i.IsDeleted = 0
                        and (b.PrivacySetting = 3 or 
                         ub.UserId = @Me)
    select count(*)
                        from zbox.Question q 
                        join zbox.box b on q.boxid = b.BoxId and b.IsDeleted = 0
                        left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where q.UserId = @Myfriend
                        and (b.PrivacySetting = 3 or ub.UserId = @Me)
   select count(*)
                        from zbox.Answer q 
                        join zbox.box b on q.boxid = b.BoxId and b.IsDeleted = 0
                        left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where q.UserId = @Myfriend
                        and (b.PrivacySetting = 3 or ub.UserId = @Me)
  select count(*)
                        from zbox.Quiz q 
                        join zbox.box b on q.boxid = b.BoxId and b.IsDeleted = 0
                        left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where q.UserId = @Myfriend
                        and q.IsDeleted = 0
						and q.Publish = 1
                        and (b.PrivacySetting = 3 or 
                         ub.UserId = @Me)

						select count(*)
                                from zbox.userboxrel ub 
                                join zbox.userboxrel ub2 on ub.boxid = ub2.boxid
                                join zbox.users u on u.userid = ub2.userid
                                where ub.userid = @Myfriend
                                and u.userid != @Myfriend;

						
";
   }
}
