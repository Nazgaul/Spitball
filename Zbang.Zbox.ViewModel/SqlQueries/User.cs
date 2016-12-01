

namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class User
   {
       public const string UserProfileWithStats = @"
		select u.userid as Id, u.username as name, u.UserImageLarge as image,u.Online,
                            u.userReputation as score, uu.universityname as universityName, u.url as Url
                            from zbox.users u left join zbox.university uu on u.UniversityId = uu.id
                            where u.userid =@Myfriend;
    select count(*)
        from 
    zbox.UserBoxRel uFriend
    join zbox.box b on b.BoxId = uFriend.BoxId and b.IsDeleted = 0
    where uFriend.UserId = @Myfriend
    and b.Discriminator = 2 ;
		 select count(*)
                        from zbox.item i 
                        join zbox.box b on i.boxid = b.BoxId and b.IsDeleted = 0
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
                                join zbox.users u on u.userid = ub2.userid
                                where ub.userid = @Myfriend
                                and u.userid != @Myfriend;";

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

       public const string GamificationBoard = @"select UserReputation as score from zbox.users
where userid = @userId";

       #endregion
   }
}
