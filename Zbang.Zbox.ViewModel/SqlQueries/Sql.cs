namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Sql
    {


        /// <summary>
        /// Used in user page to bring friends
        /// </summary>
        public const string FriendList = @"
								select u.userid as Id,
                                u.UserName as Name ,
                                u.url as Url,
                                u.UserImageLarge as Image,
                                u.UserReputation
                                from zbox.userboxrel ub 
                                join zbox.userboxrel ub2 on ub.boxid = ub2.boxid
                                join zbox.users u on u.userid = ub2.userid
                                where ub.userid = @UserId
                                and u.userid != @UserId
                                group by u.userid ,u.UserName  ,u.UserImageLarge,u.UserReputation ,u.url
                                order by u.UserReputation desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";



        public const string UniversityInfo = @"select 
uWrap.id as id,
uWrap.universityName as Name,
uWrap.LargeImage as Logo,
uWrap.BackgroundImage as Cover,
uWrap.NoOfBoxes as Boxes,
uWrap.NoOfUsers as Users,
uWrap.NoOfItems + uWrap.NoOfQuizzes as Items,
uWrap.VideoBackgroundColor as BtnColor,
uWrap.VideoFontColor as BtnFontColor,
uWrap.HeaderBackgroundColor as StripColor
from zbox.university uWrap  
where uWrap.Id = @UniversityId";


       

        


        /// <summary>
        /// Mobile api - bring user comment and reply activity in user screen
        /// </summary>
        public const string UserQuestionAndAnswersActivityMobileApi = @"select t.BoxId as BoxId, t.Text as Content, t.BoxName as boxName, t.QuestionId as Id,t.CreationTime,  t.Type, t.PostId, t.Url from (
	select b.boxid, q.Text, b.BoxName,q.QuestionId,'comment' as Type, q.CreationTime, null as PostId, b.Url
                          from zbox.Question q
                         join zbox.box b on b.BoxId = q.BoxId and b.IsDeleted = 0
                        where q.UserId = @Myfriend
                        and q.IsSystemGenerated = 0
                        and q.text is not null
                        and b.Discriminator = 2 
					union 
			select b.BoxId, a.Text,b.BoxName,a.AnswerId,'reply', a.CreationTime, a.QuestionId ,b.Url
                 from zbox.Answer a
                 join zbox.box b on b.BoxId = a.BoxId and b.IsDeleted = 0
                 where a.UserId = @Myfriend
                 and a.text is not null
                 and b.Discriminator = 2 
				 ) as t
				 order by t.CreationTime desc
  offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        

        public const string RecommendedCourses =
            @"select top(3) b.BoxId, b.BoxName as Name,b.CourseCode,b.ProfessorName as professor,
b.MembersCount,b.ItemCount , b.url,
b.MembersCount+b.ItemCount+(DATEDIFF(MINUTE,'20120101 05:00:00:000', b.UpdateTime)/(DATEDIFF(MINUTE,'20120101 05:00:00:000', GETUTCDATE())/45)) as rank  
from zbox.Box b
where b.isdeleted = 0
and b.boxid not in (select boxid from zbox.UserBoxRel ub where userid = @UserId)
and b.discriminator = 2
and b.University = @UniversityId
order by rank desc";


        public const string UniversityLeaderBoard = @"
select top(3) u.userid as id, u.UserImageLarge as image, coalesce(u.FirstName,u.username)  as name, u.UserReputation as score, u.url as url
from zbox.Users u 
where u.UniversityId = @UniversityId
and u.usertype <> 1
and userreputation > 0
 order by userreputation desc";


        public const string UserAuthenticationDetail =
    @"select 
u.UserId as Id, 
u.UserName as Name, 
u.UserImageLarge as Image,
u.Sex as Sex,
u.Url as Url,
u.Culture as Culture,
u.UserReputation as Score,
uu.UniversityName as UniversityName,
uu.Country as UniversityCountry,
uu.id as UniversityId,
case when u.UserReputation >= uu.AdminScore or usertype = 1 then 1 else 0 end  as isAdmin,
u.Email as Email,
u.CreationTime as DateTime
from zbox.Users u 
left join zbox.University uu on u.UniversityId = uu.Id
where u.userid = @UserId";


   

        public const string GetUserByMembershipId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.UserReputation as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.MembershipUserId = @MembershipUserId";

        public const string GetUserByFacebookId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.UserReputation as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.FacebookUserId = @FacebookUserId
	and u.IsEmailVerified = 1";

        public const string GetUserByGoogleId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.UserReputation as Score,
    uu.UniversityName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.GoogleUserId = @GoogleUserId
	and u.IsEmailVerified = 1";

        public const string GetUserById = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImageLarge as Image, u.Email as Email,
    u.UserReputation as Score,
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
    u.UserReputation as Score,
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

        public const string GetCoursesPageBoxes_en = @"select top 16 b.boxid as id,
                                b.BoxName as Name,
                                b.ItemCount,
                                b.MembersCount as MembersCount,
                                b.CourseCode,
                                b.ProfessorName,
                                b.Url as Url
                                from (
								select 
								b.boxid,
    b.BoxName,
	b.quizcount + b.itemcount + b.FlashcardCount as ItemCount,
	b.CourseCode as CourseCode,
	b.MembersCount as MembersCount,
	b.ProfessorName,
	b.Url as Url,
	Rank() over (partition BY libraryid order by  b.MembersCount + (b.ItemCount + b.FlashcardCount + b.QuizCount + b.CommentCount) / 4 desc,b.updatetime desc) as x
	from zbox.box b join zbox.university u on b.university=u.id
	where Discriminator = 2
	and MembersCount > 5
	and b.isdeleted = 0
	AND country='US' 
	 and id!=170400
								) b
								where x = 1
order by ItemCount desc ;";


        public const string GetCoursesPageBoxes_he = @"select top 16 b.boxid as id,
                                b.BoxName as Name,
                                b.quizcount + b.itemcount as ItemCount,
                                b.MembersCount as MembersCount,
                                b.CourseCode,
                                b.ProfessorName,
                                b.Url as Url
                                from zbox.box b join zbox.university u on b.university=u.id
                                where  MembersCount >20
                                and b.[UpdateTime]> getdate()-30 
                                and b.Discriminator = 2
                                and ItemCount> 20
                                and CommentCount> 10 
                                and b.[IsDeleted]=0 
                                and country='IL' 
                                and id!=170460 
                                order by b.[UpdateTime];";


        public const string MarketingEmailQuery = @"select email from zbox.users u join zbox.university uu on u.universityid = uu.id
and uu.country = 'IL'
and u.emailsendsettings = 0
and (u.creationtime>'2015' or u.[LastAccessTime] >'2015')
and right(userid,1)<>1
order by u.userid
offset @pageNumber*50 ROWS
FETCH NEXT 50 ROWS ONLY;";

    }
}