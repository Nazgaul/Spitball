namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Sql
    {

        public const string GetUniversityDataByUserId = @"  select  uWrap.Id as Id, 
                          coalesce (uWrap.OrgName, uWrap.UniversityName)  as Name, uWrap.LargeImage as Image,
                            uWrap.UniversityName as UniversityName,
                            uWrap.WebSiteUrl,
                            uWrap.MailAddress,
                            uWrap.FacebookUrl,
                            uWrap.TwitterUrl,
                            uWrap.TwitterWidgetId,
                            uWrap.YouTubeUrl,
                            uWrap.LetterUrl,
                            uWrap.NoOfBoxes as BoxesCount,
							uWrap.NoOfItems + uWrap.NoOfQuizzes as ItemCount,
                            uWrap.NoOfUsers as MemberCount
                            from zbox.University uWrap  
                            where 
                             uWrap.Id =@UniversityWrapper";


        /// <summary>
        /// Used in user page to bring friends
        /// </summary>
        public const string FriendList = @"
								select u.userid as Id,u.UserName as Name ,u.url as Url,
                                u.UserImageLarge as LargeImage,
                                u.UserReputation
                                from zbox.userboxrel ub 
                                join zbox.box b on ub.boxid  = b.boxid
                                join zbox.userboxrel ub2 on b.boxid = ub2.boxid
                                join zbox.users u on u.userid = ub2.userid
                                where ub.userid = @UserId
                                and b.isdeleted = 0
                                and ub2.userid != @UserId
                                group by u.userid ,u.UserName  ,u.UserImage ,u.UserImageLarge,u.UserReputation ,u.url
                                order by u.UserReputation desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

       

        public const string DashboardInfo = @"select  coalesce( uWrap.OrgName , uWrap.universityName) as Name 
, uWrap.universityName as UniName,
uWrap.LargeImage as Img
from zbox.university uWrap  
where uWrap.Id = @UniversityId";

       
        /// <summary>
        /// Used in user page to get boxes common with current user and his friend
        /// </summary>
        public const string UserWithFriendBoxes = @"select 
b.boxid as id,
b.BoxName as boxName,
        COALESCE( uMe.UserType,0) as userType,
        b.quizcount + b.itemcount as ItemCount,
        b.MembersCount as MembersCount,
        b.commentcount as CommentCount,
        b.CourseCode,
        b.ProfessorName,
        b.Discriminator as boxType,
        b.Url as Url
        from 
    zbox.UserBoxRel uFriend
    join zbox.box b on b.BoxId = uFriend.BoxId and b.IsDeleted = 0
    left join zbox.UserBoxRel uMe on b.BoxId = uMe.BoxId and uMe.UserId = @Me
    where uFriend.UserId = @Myfriend
    and (b.PrivacySetting = 3 or uMe.UserId = @Me)
	ORDER BY b.UpdateTime desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        /// <summary>
        /// Used in user page to get files common with current user
        /// </summary>
        public const string UserWithFriendFiles = @"select i.ItemId as id,
i.blobname as image, 
i.Rate as rate,
i.NumberOfViews as numOfViews,
i.Name as name,
b.boxid as boxid, 
b.boxname as boxname,
i.url as Url,
i.Discriminator as Type
                        from zbox.item i 
                        join zbox.box b on i.boxid = b.BoxId and b.IsDeleted = 0
                        left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where i.UserId = @Myfriend
                        and i.IsDeleted = 0
                        and (b.PrivacySetting = 3 or 
                         ub.UserId = @Me)
                        order by i.itemid desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        /// <summary>
        ///  Used in user page to get question common with current user
        /// </summary>
        public const string UserWithFriendQuestion = @" select b.BoxName as boxName,q.Text as content, b.BoxId as boxId,
                        (select count(*) from zbox.Answer a where a.QuestionId = q.QuestionId) as answersCount,
						b.url as Url, q.QuestionId as id
                          from zbox.Question q
                         join zbox.box b on b.BoxId = q.BoxId and b.IsDeleted = 0
                         left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where q.UserId = @Myfriend
                        and q.IsSystemGenerated = 0
                        and (b.PrivacySetting = 3 or 
                         ub.UserId = @Me)
                    order by q.QuestionId desc
  offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        /// <summary>
        ///  Used in user page to get answers common with current user
        /// </summary>
        public const string UserWithFriendAnswer = @"select b.BoxId as boxId, b.BoxName as boxName, q.UserId as qUserId, q.Text as qContent, 
                   uQuestion.UserImage as qUserImage, uQuestion.UserName as qUserName, a.Text as Content, 
                   (select count(*) from zbox.Answer a where a.QuestionId = q.QuestionId) as answersCount,
				  b.url as Url , a.AnswerId as Id
                 from zbox.Answer a
                 join zbox.Question q on a.QuestionId = q.QuestionId
                 join zbox.Users uQuestion on uQuestion.UserId = q.UserId
                 join zbox.box b on b.BoxId = a.BoxId and b.IsDeleted = 0
                 left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                 where a.UserId = @Myfriend
                 and (b.PrivacySetting = 3 or  ub.UserId = @Me)
                 order by a.AnswerId desc
  offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";


        /// <summary>
        /// Mobile api - bring user comment and reply acitivity in user screen
        /// </summary>
        public const string UserQuestionAndAnswersActivityMobileApi = @"select t.BoxId as BoxId, t.Text as Content, t.BoxName as boxName, t.QuestionId as Id,  t.Type, t.PostId from (
	select b.boxid, q.Text, b.BoxName,q.QuestionId,'comment' as Type, q.CreationTime, null as PostId
                          from zbox.Question q
                         join zbox.box b on b.BoxId = q.BoxId and b.IsDeleted = 0
                         left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where q.UserId = @Myfriend
                        and q.IsSystemGenerated = 0
                        and (b.PrivacySetting = 3 or 
                         ub.UserId = @Me)
					union 
			select b.BoxId, a.Text,b.BoxName,a.AnswerId,'reply', a.CreationTime, a.QuestionId 
                 from zbox.Answer a
                 join zbox.box b on b.BoxId = a.BoxId and b.IsDeleted = 0
                 left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                 where a.UserId = @Myfriend
                 and (b.PrivacySetting = 3 or  ub.UserId = @Me)
				 ) as t
				 order by t.CreationTime desc
  offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        /// <summary>
        ///  Used in user page to get user invites
        /// </summary>
        public const string UserPersonalInvites = @"
   select i.Image as userImage, i.UserName as username, 
 null as boxName,null as boxid, i.email,
 i.IsUsed as status, i.TypeOfMsg as InviteType
   from zbox.Invite i
   where senderid =@Me
   and TypeOfMsg = 3
union all
select u.UserImageLarge as userImage, u.UserName as username, 
 b.BoxName as boxName,b.boxid as boxid, i.email,
 i.IsUsed as status, i.TypeOfMsg as InviteType
   from zbox.Invite i
   inner join zbox.UserBoxRel ub on i.UserBoxRelId = ub.UserBoxRelId
   inner join zbox.Box b on ub.BoxId = b.BoxId
   inner join zbox.Users u on ub.UserId = u.UserId
   where senderid =@Me
   and TypeOfMsg = 2
";


        public const string UserInvites = @"
select id as MsgId, u.UserImage as userpic,
 u.UserName as username, 
 i.CreationTime as date,
 b.BoxName,
b.Url ,  isRead as [IsRead]
from zbox.UserBoxRel ub
join zbox.Invite i on ub.UserBoxRelId = i.UserBoxRelId and i.IsUsed = 0  and i.isdeleted = 0
join zbox.Users u on i.SenderId = u.UserId
join zbox.box b on ub.BoxId = b.BoxId and b.isdeleted = 0
where ub.UserType = 1
and ub.UserId = @userid
order by isRead asc, i.CreationTime desc ";

        public const string RecommendedCourses =
            @"select top(3) b.BoxId, b.BoxName as Name,b.CourseCode,b.ProfessorName as professor,
b.MembersCount,b.ItemCount , b.url,
b.MembersCount+b.ItemCount+(DATEDIFF(MINUTE,'20120101 05:00:00:000', b.UpdateTime)/(DATEDIFF(MINUTE,'20120101 05:00:00:000', GETUTCDATE())/45)) as rank  
from zbox.Box b
where b.isdeleted = 0
and b.boxid not in (select boxid from zbox.UserBoxRel ub where userid = @UserId)
and b.University = @UniversityId
order by rank desc";


        public const string UniversityLeaderBoard = @"
select top(3) u.userid as id, u.UserImageLarge as image, u.username as name, u.UserReputation as score, u.url as url
from zbox.Users u 
where u.UniversityId = @UniversityId
and u.usertype <> 1
and userreputation > 0
 order by userreputation desc";
     

        public const string UserAuthenticationDetail =
    @"select u.UserId as Id, u.UserName as Name, u.UserImage as Image,
     u.FirstTimeDashboard as FirstTimeDashboard,
     u.FirstTimeLibrary as FirstTimeLibrary,
     u.FirstTimeItem as FirstTimeItem,
     u.FirstTimeBox as FirstTimeBox, 
     u.Url as Url,
     u.Culture as Culture,
     u.UserReputation as Score,
     uu.UniversityName as UniversityName,
     uu.Country as UniversityCountry,
	 uu.id as UniversityId,
     case when u.UserReputation >= uu.AdminScore or usertype = 1 then 1 else 0 end  as isAdmin
     from zbox.Users u 
	 left join zbox.University uu on u.UniversityId = uu.Id
     where u.userid = @UserId";

        public const string GetUserByMembershipId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImage as Image, u.Email as Email,
    u.FirstTimeDashboard as FirstTimeDashboard,
    u.FirstTimeLibrary as FirstTimeLibrary,
    u.FirstTimeItem as FirstTimeItem,
    u.FirstTimeBox as FirstTimeBox, 
    u.UserReputation as Score,
    uu.OrgName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.MembershipUserId = @MembershipUserId";

        public const string GetUserByFacebookId = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImage as Image, u.Email as Email,
    u.FirstTimeDashboard as FirstTimeDashboard,
    u.FirstTimeLibrary as FirstTimeLibrary,
    u.FirstTimeItem as FirstTimeItem,
    u.FirstTimeBox as FirstTimeBox, 
    u.UserReputation as Score,
    uu.OrgName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.FacebookUserId = @FacebookUserId
	and u.IsEmailVerified = 1";

        public const string GetUserById = @" select u.UserId as Id, u.UserName as Name, u.Culture as Culture, 
    u.UserImage as Image, u.Email as Email,
    u.FirstTimeDashboard as FirstTimeDashboard,
    u.FirstTimeLibrary as FirstTimeLibrary,
    u.FirstTimeItem as FirstTimeItem,
    u.FirstTimeBox as FirstTimeBox, 
    u.UserReputation as Score,
    uu.OrgName as LibName,
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
    u.UserImage as Image,
    u.Email as Email,
    u.FirstTimeDashboard as FirstTimeDashboard,
    u.FirstTimeLibrary as FirstTimeLibrary,
    u.FirstTimeItem as FirstTimeItem,
    u.FirstTimeBox as FirstTimeBox, 
    u.UserReputation as Score,
    uu.OrgName as LibName,
    uu.LargeImage as LibImage,
    uu.Id as UniversityId,
	uu.universityData as UniversityData
    from zbox.Users u left join zbox.University uu on u.UniversityId = uu.Id
    where u.Email = @Email
	and (u.MembershipUserId is not null or u.FacebookUserId is not null)";
    }
}
