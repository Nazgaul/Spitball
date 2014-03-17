using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Sql
    {
        public const string GetUniversitiesList = @" select u.userid as Uid , 
                        coalesce(AliasName,userName) as Name,
                        u.userimage as Image,u.NeedCode as NeedCode, u.Country as Country,
                        (select count(*) from zbox.users where universityid2 = u.userid) as MemberCount
                        from zbox.users u 
                        where u.usertype = 1 
                        order by MemberCount desc";
        public const string GetWallList = @"select top(50) userName as UserName, userimage as UserImage,userid as UserId,boxid as BoxId,boxname as BoxName,action as Action, universityname as uniName
                                    from (	 
	                                       select 
	                                        author.userName, author.UserImage, author.userid as userid, b.boxid as boxid, b.boxname,i.CreationTime as date, 'item' as action,
 case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								                    else null
								                    end as universityname

	                                        from  zbox.UserBoxRel ub 
                                           join zbox.Box b on b.BoxId = ub.BoxId and b.IsDeleted = 0
                                           join zbox.Item i on b.boxid = i.BoxId and i.IsDeleted = 0
                                           join zbox.Users author on author.UserId = i.UserId
                                           where ub.UserId = @UserId and author.userid != @UserId 
	   
	                                       union all
	                                       select   author.userName, author.UserImage, author.userid as userid, b.boxid as boxid, b.boxname,q.CreationTime as date,'question' as action,
 case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								                    else null
								                    end as universityname
	                                        from  zbox.UserBoxRel ub 
                                           join zbox.Box b on b.BoxId = ub.BoxId and b.IsDeleted = 0
	                                       join zbox.question q on q.boxid = b.boxid
	                                       join zbox.Users author on author.UserId = q.UserId
	                                       where ub.UserId =@UserId  and author.userid != @UserId 
	  
	                                        union all 
	                                       select   author.userName, author.UserImage, author.userid as userid, b.boxid as boxid, b.boxname,q.CreationTime as date,'answer' as action,
 case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								                    else null
								                    end as universityname

	                                        from  zbox.UserBoxRel ub 
                                           join zbox.Box b on b.BoxId = ub.BoxId and b.IsDeleted = 0
	                                       join zbox.answer q on q.boxid = b.boxid
	                                       join zbox.Users author on author.UserId = q.UserId
	                                       where ub.UserId = @UserId  and author.userid != @UserId
	                                      ) t
	                                      order by t.date desc;";
        public const string GetUniversityDataByUserId = @" select  uu.UserId as Id, uWrap.userName as Name, uWrap.UserImageLarge as Image,
                            uWrap.WebSiteUrl,
                            uWrap.MailAddress,
                            uWrap.FacebookUrl,
                            uWrap.TwitterUrl,
                            uWrap.TwitterWidgetId,
                            uWrap.YouTubeUrl,
                            uWrap.LetterUrl,
                            (select count(*) from zbox.Box b 
                            where b.OwnerId = uu.UserId and b.Discriminator = 2 and b.IsDeleted = 0) as BoxesCount,
                            (select sum(itemcount) from zbox.Box b 
                            where b.OwnerId = uu.UserId and b.Discriminator = 2 and b.IsDeleted = 0) as ItemCount,
                            (select count(*) from zbox.Users u where u.UniversityId2 in ( uu.UserId , uWrap.userid)) as MemberCount
                            from zbox.Users uu , zbox.users uWrap  
                            where uu.UserId = @UserId
                            and uWrap.UserId = @UniversityWrapper";

        /// <summary>
        /// Used in user page to bring friends
        /// </summary>
        public const string FriendList = @"select u.userid as Uid,u.UserName as Name,u.UserImage as Image ,
                                u.UserImageLarge as LargeImage,
                                u.UserReputation
								from zbox.Users u where userid =( select universityid2 from zbox.users where userid = @userid)
								union 
								select u.userid as Uid,u.UserName as Name ,u.UserImage as Image ,
                                u.UserImageLarge as LargeImage,
                                u.UserReputation
                                from zbox.userboxrel ub 
                                join zbox.box b on ub.boxid  = b.boxid
                                join zbox.userboxrel ub2 on b.boxid = ub2.boxid
                                join zbox.users u on u.userid = ub2.userid
                                where ub.userid = @UserId
                                and b.isdeleted = 0
                                and ub2.userid != @UserId
                                group by u.userid ,u.UserName  ,u.UserImage ,u.UserImageLarge,u.UserReputation
                                order by u.UserReputation desc;";


        public const string UserBoxes = @"select b.boxid as id,
                                b.BoxName,
                                b.BoxPicture as BoxPicture,
                                ub.UserType, 
                                b.itemcount as ItemCount,
                                b.MembersCount as MembersCount,
                                b.commentcount as CommentCount,
                                b.CourseCode,
                                b.ProfessorName,
								
                                b.Discriminator as boxType,
								case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								else null
								end as universityname
                                  from Zbox.box b join zbox.UserBoxRel ub on b.BoxId = ub.BoxId  
                                  where 
                                  b.IsDeleted = 0   
                                  and ub.UserId = @UserId
                                  ORDER BY ub.UserBoxRelId desc;";

       
        /// <summary>
        /// Used in user page to get boxes common with current user and his friend
        /// </summary>
        public const string UserWithFriendBoxes = @"select COALESCE( uMe.UserType,0) as userType, b.boxid as id ,b.BoxName as name,b.BoxPicture as picture,
        case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								else null
								end as universityname
                            from 
                        zbox.UserBoxRel uFriend
                        join zbox.box b on b.BoxId = uFriend.BoxId and b.IsDeleted = 0
                        left join zbox.UserBoxRel uMe on b.BoxId = uMe.BoxId and uMe.UserId = @Me
                        where uFriend.UserId = @Myfriend
                        and (b.PrivacySetting = 3 or uMe.UserId = @Me);";

        /// <summary>
        /// Used in user page to get files common with current user
        /// </summary>
        public const string UserWithFriendFiles = @" select i.ItemId as id, i.ThumbnailBlobName as image, i.Rate as rate,i.NumberOfViews as numOfViews,i.Name as name,b.boxid as boxid, b.boxname as boxname,
  case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								else null
								end as universityname
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
        public const string UserWithFriendQuestion = @" select b.BoxPicture as boxPicutre,b.BoxName as boxName,q.Text as content, b.BoxId as boxid,
                        (select count(*) from zbox.Answer a where a.QuestionId = q.QuestionId) as answersCount,
						case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								else null
								end as universityname
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
        public const string UserWithFriendAnswer = @"select b.BoxPicture as boxPicture, b.BoxId as boxid, b.BoxName as boxName, q.UserId as qUserId, q.Text as qContent, 
                   uQuestion.UserImage as qUserImage, uQuestion.UserName as qUserName, a.Text as Content, 
                   (select count(*) from zbox.Answer a where a.QuestionId = q.QuestionId) as answersCount,
				   case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								else null
								end as universityname
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
                m.TypeOfMsg as inviteType,b.BoxName as boxName,b.BoxPicture as boxPicture,b.boxid as boxid,
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
    }
}
