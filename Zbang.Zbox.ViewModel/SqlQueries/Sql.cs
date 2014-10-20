namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Sql
    {

        public const string GetWallList = @"
select top(50) userName as UserName, userimage as UserImage,userid as UserId,boxid as BoxId,boxname as BoxName,action as Action,  Url as Url
                                    from (	 
	                                       select 
	                                        author.userName, author.UserImage, author.userid as userid, b.boxid as boxid, b.boxname,i.CreationTime as date, 'item' as action,
 b.Url as Url

	                                        from  zbox.UserBoxRel ub 
                                           join zbox.Box b on b.BoxId = ub.BoxId and b.IsDeleted = 0
                                           join zbox.Item i on b.boxid = i.BoxId and i.IsDeleted = 0
                                           join zbox.Users author on author.UserId = i.UserId
                                           where ub.UserId = @UserId and author.userid != @UserId 
	   
	                                       union all
	                                       select   author.userName, author.UserImage, author.userid as userid, b.boxid as boxid, b.boxname,q.CreationTime as date,'question' as action,
 b.Url as Url
	                                        from  zbox.UserBoxRel ub 
                                           join zbox.Box b on b.BoxId = ub.BoxId and b.IsDeleted = 0
	                                       join zbox.question q on q.boxid = b.boxid
	                                       join zbox.Users author on author.UserId = q.UserId
	                                       where ub.UserId =@UserId  and author.userid != @UserId 
	  
	                                        union all 
	                                       select   author.userName, author.UserImage, author.userid as userid, b.boxid as boxid, b.boxname,q.CreationTime as date,'answer' as action,
 b.Url as Url

	                                        from  zbox.UserBoxRel ub 
                                           join zbox.Box b on b.BoxId = ub.BoxId and b.IsDeleted = 0
	                                       join zbox.answer q on q.boxid = b.boxid
	                                       join zbox.Users author on author.UserId = q.UserId
	                                       where ub.UserId = @UserId  and author.userid != @UserId
	                                      ) t
	                                      order by t.date desc;";
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
                            (select sum(itemcount) from zbox.Box b 
                            where b.University = uWrap.Id and b.Discriminator = 2 and b.IsDeleted = 0) as ItemCount,
                            (select count(*) from zbox.Users u where u.UniversityId = uWrap.Id) as MemberCount
                            from zbox.University uWrap  
                            where 
                             uWrap.Id =@UniversityWrapper";


        /// <summary>
        /// Used in user page to bring friends
        /// </summary>
        public const string FriendList = @"
								select u.userid as Id,u.UserName as Name ,u.UserImage as Image ,u.url as Url,
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
                                order by u.UserReputation desc;";


        public const string UserBoxes = @"select b.boxid as id,
                                b.BoxName,
                                b.pictureUrl as BoxPicture,
                                ub.UserType, 
                                b.quizcount + b.itemcount as ItemCount,
                                b.MembersCount as MembersCount,
                                b.commentcount as CommentCount,
                                b.CourseCode,
                                b.ProfessorName,
								
                                b.Discriminator as boxType,
								b.Url as Url
                                  from Zbox.box b join zbox.UserBoxRel ub on b.BoxId = ub.BoxId  
                                  where 
                                  b.IsDeleted = 0   
                                  and ub.UserId = @UserId
                                  ORDER BY ub.UserBoxRelId desc;";

       
        /// <summary>
        /// Used in user page to get boxes common with current user and his friend
        /// </summary>
        public const string UserWithFriendBoxes = @"select COALESCE( uMe.UserType,0) as userType, b.boxid as id ,b.BoxName as name,b.pictureurl as picture,
        b.Url as Url
                            from 
                        zbox.UserBoxRel uFriend
                        join zbox.box b on b.BoxId = uFriend.BoxId and b.IsDeleted = 0
                        left join zbox.UserBoxRel uMe on b.BoxId = uMe.BoxId and uMe.UserId = @Me
                        where uFriend.UserId = @Myfriend
                        and (b.PrivacySetting = 3 or uMe.UserId = @Me);";

        /// <summary>
        /// Used in user page to get files common with current user
        /// </summary>
        public const string UserWithFriendFiles = @" select i.ItemId as id, i.ThumbnailUrl as image, i.Rate as rate,i.NumberOfViews as numOfViews,i.Name as name,b.boxid as boxid, b.boxname as boxname,
                        i.url as Url
                        from zbox.item i 
                        join zbox.box b on i.boxid = b.BoxId and b.IsDeleted = 0
                        left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where i.UserId = @Myfriend
                        and i.IsDeleted = 0
                        and (b.PrivacySetting = 3 or 
                         ub.UserId = @Me)
                        order by i.itemid desc;";

        /// <summary>
        ///  Used in user page to get question common with current user
        /// </summary>
        public const string UserWithFriendQuestion = @" select b.pictureurl as boxPicutre,b.BoxName as boxName,q.Text as content, b.BoxId as boxid,
                        (select count(*) from zbox.Answer a where a.QuestionId = q.QuestionId) as answersCount,
						b.url as Url
                          from zbox.Question q
                         join zbox.box b on b.BoxId = q.BoxId and b.IsDeleted = 0
                         left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                        where q.UserId = @Myfriend

                        and (b.PrivacySetting = 3 or 
                         ub.UserId = @Me)
                    order by q.QuestionId desc;";

        /// <summary>
        ///  Used in user page to get answers common with current user
        /// </summary>
        public const string UserWithFriendAnswer = @"select b.pictureurl as boxPicture, b.BoxId as boxid, b.BoxName as boxName, q.UserId as qUserId, q.Text as qContent, 
                   uQuestion.UserImage as qUserImage, uQuestion.UserName as qUserName, a.Text as Content, 
                   (select count(*) from zbox.Answer a where a.QuestionId = q.QuestionId) as answersCount,
				  b.url as Url
                 from zbox.Answer a
                 join zbox.Question q on a.QuestionId = q.QuestionId
                 join zbox.Users uQuestion on uQuestion.UserId = q.UserId
                 join zbox.box b on b.BoxId = a.BoxId and b.IsDeleted = 0
                 left join zbox.userboxrel ub on b.BoxId = ub.BoxId and ub.UserId = @Me
                 where a.UserId = @Myfriend
                 and (b.PrivacySetting = 3 or  ub.UserId = @Me)
                 order by a.AnswerId desc";


        /// <summary>
        ///  Used in user page to get user invites
        /// </summary>
        public const string UserPersonalInvites = @"select distinct u.UserImageLarge as userImage, u.UserName as username ,u.UserId as userid,
                m.TypeOfMsg as inviteType,b.BoxName as boxName,b.pictureurl as boxPicture,b.boxid as boxid,
                 case m.TypeOfMsg when 2 then (select count(*) from zbox.UserBoxRel ub where ub.userid = u.userid and ub.BoxId = b.BoxId)
                 when 3 then u.IsEmailVerified
                 end as status, m.MessageId
                 from 
                zbox.Message m
                join zbox.users u on m.RecepientId = u.UserId
                left join zbox.Box b on b.BoxId = m.BoxId and b.IsDeleted = 0
                where m.TypeOfMsg in (2,3)
                and SenderId = @Me
                order by status, m.MessageId desc;";


        public const string UserInvites = @"
select * from (select u.UserImage as userpic,
 u.UserName as username,
 m.MessageId as msgId,
  m.CreationTime as date,
  m.NotRead as isread,
  m.New as IsNew,
  m.Text as message,
b.BoxName,
b.BoxId,
b.Url
from zbox.message m 
inner join zbox.box b on m.BoxId = b.BoxId and b.IsDeleted = 0
inner join zbox.users u on u.UserId = m.SenderId
where m.RecepientId = @userid
 and TypeOfMsg = 2
 and isactive = 1
/* union all
select u.UserImage as userpic,
 u.UserName as username,
 m.MessageId as msgId,
  m.CreationTime as date,
  m.NotRead as isread,
  m.New as IsNew,
  m.Text as message,
null,
null,
null
 from zbox.message m 
 inner join zbox.users u on u.UserId = m.SenderId
where m.RecepientId = @userid
 and TypeOfMsg = 1*/ ) t
 order by t.msgid desc";

        public const string RecommendedCourses =
            @"select top(3) b.BoxName as Name,b.CourseCode,b.ProfessorName as professor,
b.PictureUrl as Picture,b.MembersCount,b.ItemCount , b.url,
b.MembersCount+b.ItemCount+(DATEDIFF(MINUTE,'20120101 05:00:00:000', b.UpdateTime)/(DATEDIFF(MINUTE,'20120101 05:00:00:000', GETUTCDATE())/45)) as rank  
from zbox.Box b
where b.isdeleted = 0
and b.discriminator = 2
order by rank desc";

        public const string UserAuthenticationDetail =
    @"select u.UserId as Id, u.UserName as Name, u.UserImage as Image,
     u.FirstTimeDashboard as FirstTimeDashboard,
     u.FirstTimeLibrary as FirstTimeLibrary,
     u.FirstTimeItem as FirstTimeItem,
     u.FirstTimeBox as FirstTimeBox, 
     u.Url as Url,
     u.Email as Email,
     u.UserReputation as Score,
     uu.OrgName as LibName,
     uu.Image as LibImage,
     case when u.UserReputation >= uu.AdminScore then 1 else 0 end as isAdmin
     from zbox.Users u 
	 left join zbox.University uu on u.UniversityId = uu.Id
     where u.userid = @UserId";
    }
}
