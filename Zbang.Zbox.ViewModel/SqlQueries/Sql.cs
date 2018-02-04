namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Sql
    {
        /// <summary>
        /// Used in user page to bring friends
        /// </summary>
        public const string FriendList = @"
								select distinct u.userid as Id,
                                u.UserName as Name ,
                                u.UserImageLarge as Image,
                                u.Score,
                                u.BadgeCount as badges
                                from zbox.userboxrel ub 
                                join zbox.userboxrel ub2 on ub.boxid = ub2.boxid
                                join zbox.users u on u.userid = ub2.userid
                                where ub.userid = @UserId
                                and u.userid != @UserId
                                order by u.Score desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        public const string UniversityInfo = @"select 
uWrap.id as id,
uWrap.universityName as Name,
uWrap.LargeImage as Logo,
uWrap.BackgroundImage as Cover,
uWrap.NoOfBoxes as Boxes,
uWrap.NoOfUsers as Users,
uWrap.NoOfItems + uWrap.NoOfQuizzes + coalesce(uWrap.NoOfFlashcards,0) as Items,
uWrap.VideoBackgroundColor as BtnColor,
uWrap.VideoFontColor as BtnFontColor,
uWrap.HeaderBackgroundColor as StripColor
from zbox.university uWrap  
where uWrap.Id = @UniversityId";

        /// <summary>
        /// Mobile api - bring user comment and reply activity in user screen
        /// </summary>
        public const string UserQuestionAndAnswersActivityMobileApi = @"select t.BoxId as BoxId, t.Text as Content, t.BoxName as boxName, t.QuestionId as Id,t.CreationTime,  t.Type, t.PostId, t.Url,t.LibraryId as DepartmentId  from (
	select b.boxid, q.Text, b.BoxName,q.QuestionId,'comment' as Type, q.CreationTime, null as PostId, b.Url,b.LibraryId
                          from zbox.Question q
                         join zbox.box b on b.BoxId = q.BoxId and b.IsDeleted = 0
                        where q.UserId = @Myfriend
                        and q.IsSystemGenerated = 0
                        and q.text is not null
                        and b.Discriminator = 2 
					union 
			select b.BoxId, a.Text,b.BoxName,a.AnswerId,'reply', a.CreationTime, a.QuestionId ,b.Url,b.LibraryId
                 from zbox.Answer a
                 join zbox.box b on b.BoxId = a.BoxId and b.IsDeleted = 0
                 where a.UserId = @Myfriend
                 and a.text is not null
                 and b.Discriminator = 2 
				 ) as t
				 order by t.CreationTime desc
  offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        public const string UserAuthenticationDetail =
    @"select 
u.UserId as Id, 
u.UserName as Name, 
u.UserImageLarge as Image,
u.Culture as Culture,
u.score as Score,
u.BadgeCount as badges,
uu.UniversityName as UniversityName,
uu.Country as UniversityCountry,
uu.id as UniversityId,
case when u.score >= uu.AdminScore or usertype = 1 then 1 else 0 end  as isAdmin,
u.Email as Email,
u.CreationTime as DateTime
from zbox.Users u 
left join zbox.University uu on u.UniversityId = uu.Id
where u.userid = @UserId;";

        public const string GetUserByMembershipId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.score as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.MembershipUserId = @MembershipUserId";

        public const string GetUserByFacebookId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.score as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.FacebookUserId = @FacebookUserId
	and u.IsEmailVerified = 1";

        public const string GetUserByGoogleId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.score as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.GoogleUserId = @GoogleUserId
	and u.IsEmailVerified = 1";

        public const string GetUserById = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.score as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.UserId = @UserId
	and u.IsEmailVerified = 1";

        public const string GetUserByEmail = @"
select 
u.UserId as Id, 
u.UserName as Name,
u.Culture as Culture,
u.googleuserid as GoogleId, 
u.facebookuserid as FacebookId,
u.MembershipUserId as membershipId,
    u.UserImageLarge as Image,
    u.Email as Email,
    u.score as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.Email = @Email
	and (u.MembershipUserId is not null or u.FacebookUserId is not null or u.GoogleUserId is not null)";

        public const string GetUserDetailsForgotPassword = @"select 
u.FirstName as FirstName,
u.Culture as Culture, 
u.MembershipUserId as IdentityId,
u.FacebookUserId as FacebookId,
u.GoogleUserId as GoogleId
    from zbox.Users u 
    where u.Email = @Email";

        public const string GetUserAccountData = @"  select 
    u.FirstName as FirstName,
    u.LastName as LastName,
    u.UserImageLarge as Image, 
    v.UniversityName as University,
    v.LargeImage as UniversityImage,
    u.Email as Email,
    u.Culture as Language,
    case when u.MembershipUserId is null then 0 else 1 end  as System
    from zbox.Users u left join zbox.University v on u.UniversityId = v.Id
    where u.userId = @UserId";

     

        public const string LocationByIp = @"select country_code2 from zbox.ip_range
	where ip_from <= @IP and @IP <= ip_to";


    }
}