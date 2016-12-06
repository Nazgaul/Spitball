﻿

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class User
    {
        //    select count(*)
        //    from
        //zbox.UserBoxRel uFriend
        //join zbox.box b on b.BoxId = uFriend.BoxId and b.IsDeleted = 0
        //where uFriend.UserId = @Myfriend
        //and b.Discriminator = 2 ;
        public const string UserProfileWithStats = @"
		select u.userid as Id, u.username as name, u.UserImageLarge as image,u.Online,
                            u.score as score, uu.universityname as universityName, u.url as Url
                            from zbox.users u left join zbox.university uu on u.UniversityId = uu.id
                            where u.userid =@Myfriend;

		 select count(*)
                        from zbox.item i 
                        join zbox.box b on i.boxid = b.BoxId
                        where i.UserId = @Myfriend
                        and i.IsDeleted = 0
                        and b.Discriminator = 2;
    select count(*)
                        from zbox.Question q 
                        join zbox.box b on q.boxid = b.BoxId and b.IsDeleted = 0
                        where q.UserId = @Myfriend
                        and q.IsSystemGenerated = 0
						and q.text is not null
                        and b.Discriminator = 2;
   select count(*)
                        from zbox.Answer q 
                        join zbox.box b on q.boxid = b.BoxId and b.IsDeleted = 0
                        where q.UserId = @Myfriend
                        and q.text is not null
                        and b.Discriminator =2;
  select count(*)
                        from zbox.Quiz q 
                        join zbox.box b on q.boxid = b.BoxId and b.IsDeleted = 0
                        where q.UserId = @Myfriend
                        and q.IsDeleted = 0
						and q.Publish = 1
                        and b.Discriminator = 2;

						select count(*)
                                from zbox.userboxrel ub 
                                join zbox.userboxrel ub2 on ub.boxid = ub2.boxid
                                where ub.userid = @Myfriend
                                and ub2.userid != @Myfriend;
";

        /// <summary>
        /// Used in user page to get boxes common with current user and his friend
        /// </summary>
        public const string UserWithFriendBoxes = @"select b.boxid as id,
b.BoxName as Name,
        b.quizcount + b.itemcount + COALESCE(b.FlashcardCount,0) as ItemCount,
        b.MembersCount as MembersCount,
        b.CourseCode,
        b.ProfessorName,
        b.Url as Url,
b.LibraryId as departmentId
        from 
    zbox.UserBoxRel uFriend
    join zbox.box b on b.BoxId = uFriend.BoxId and b.IsDeleted = 0
    where uFriend.UserId = @Myfriend
    and b.Discriminator = 2 
	ORDER BY b.UpdateTime desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        /// <summary>
        /// Used in user page to get files common with current user
        /// </summary>
        public const string UserWithFriendFiles = @"select i.ItemId as id,
i.blobname as source, 
i.CreationTime as date,
i.LikeCount as likes,
i.Content,
i.NumberOfViews as numOfViews,
i.Name as name,
i.url as Url,
i.Discriminator as Type,
b.BoxId as boxId
                        from zbox.item i 
                        join zbox.box b on i.boxid = b.BoxId and b.IsDeleted = 0
                        where i.UserId = @Myfriend
                        and i.IsDeleted = 0
                        and b.Discriminator = 2 
                        order by i.itemid desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";

        /// <summary>
        /// Used in user page to get quizzes common with current user
        /// </summary>
        public const string UserWithFriendQuizzes = @"select q.Id as id,
q.url as Url,
q.Name as name,
q.Rate as rate,
q.NumberOfViews as numOfViews
                        from zbox.Quiz q 
                        join zbox.box b on q.boxid = b.BoxId and b.IsDeleted = 0
                        where q.UserId = @Myfriend
                        and q.IsDeleted = 0
						and q.Publish = 1
                        and b.Discriminator = 2 
                        order by q.Id desc
    offset @pageNumber*@rowsperpage ROWS
    FETCH NEXT @rowsperpage ROWS ONLY;";


        #region Gamification

        public const string GamificationBoard = @"select score,BadgeCount from zbox.users where userid = @userId";
        public const string Level = @"select score from zbox.users where userid = @userId";
        public const string Badge = @"select name as badge,progress from zbox.Badge where userid = @userId";
//        public const string LeaderBoard = @"
//with leaderboard as (
//select userid as id,username as name,score,UserImageLarge as image,
//ROW_NUMBER () over (partition BY universityid order by score desc) as location
// from zbox.Users  u
// where universityid = @UniversityId
// )
// SELECT *
//FROM leaderboard
//WHERE location IN (SELECT location+i
//             FROM leaderboard
//             CROSS JOIN (
//			 SELECT -1 AS i 
//			 UNION ALL SELECT -4 
//			 UNION ALL SELECT -2
//			 UNION ALL SELECT -3
//			 UNION ALL SELECT -5
//			 UNION ALL SELECT -6
//			 UNION ALL SELECT -7
//			 UNION ALL SELECT -8
//			 UNION ALL SELECT -9
//			 UNION ALL SELECT -10
//			 UNION ALL SELECT 0 
//			 UNION ALL SELECT 1
//			 UNION ALL SELECT 2
//			 UNION ALL SELECT 3
//			 UNION ALL SELECT 4
//			 UNION ALL SELECT 5
//			 UNION ALL SELECT 6
//			 UNION ALL SELECT 7
//			 UNION ALL SELECT 8
//			 UNION ALL SELECT 9
//			 UNION ALL SELECT 10
//			 ) n
//             WHERE id = @UserId
//			 );";

        public const string LeaderBoard =
            @"select userid as id,username as name,score,UserImageLarge as image from zbox.users 
where universityid = (select UniversityId from zbox.users where userid = @UserId)
order by score desc";

        #endregion
    }
}
