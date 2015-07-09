namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Email
    {

        public const string GetUserListByNotificationSettings =
            @"select distinct u.userid as UserId,u.email as Email, u.culture as Culture, u.UserName as UserName
      from zbox.userboxrel ub 
      join zbox.users u on ub.userid = u.userid
      join zbox.box b on ub.boxid = b.boxid
      where notificationSettings = @Notification
	  and ub.UserType in (2,3)
      and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@NotificationTime,b.updateTime)) > 0;";

        public const string GetBoxPossibleUpdateByUser =
            @" select distinct b.boxid as BoxId, b.boxname as BoxName, 
    (select universityname from zbox.university u where b.university = u.Id) as UniversityName, b.Url
    from zbox.userboxrel ub 
    join zbox.box b on ub.boxid = b.boxid and b.isdeleted = 0
    where
	ub.userid = @UserId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,b.updateTime)) >0;";

        public const string GetItemUpdateByBox =
            @" select u.username as UserName,u.userid as UserId, i.name as Name,
    i.blobname as Picture , i.Url
    from zbox.item i inner join zbox.users u on u.userid = i.userid
    where  i.IsDeleted = 0
    and i.boxid = @BoxId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,i.creationTime)) >0;";

        public const string GetQuizUpdateByBox = @"select u.username as UserName,u.userid as UserId, q.name as Name, 
    'http://az32006.vo.msecnd.net/mailcontainer/Quiz.jpg' as Picture, q.url
	 from zbox.quiz q inner join zbox.users u on u.userid = q.userid
	 where q.boxid = @BoxId
     and publish = 1
     and isdeleted = 0
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,q.creationTime)) > 0;";

        public const string GetQuestionUpdateByBox = @"select
    u.userName as UserName,
    u.userid as UserId,
    u.UserImageLarge as UserImage,
    q.Text as Text
    from zbox.question q 
    join Zbox.Users u on q.UserId = u.userid
    where q.boxid = @BoxId
	and q.text is not null
    and q.IsSystemGenerated = 0
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,q.creationTime)) >0;";

        public const string GetAnswerUpdateByBox = @" 
    select u.userName as UserName,
    u.userid as UserId,
    u.UserImageLarge as UserImage,
    a.Text as Text
    from zbox.answer a 
    join Zbox.Users u on a.UserId = u.userid
    where a.boxid = @BoxId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,a.creationTime)) >0;";

        public const string GetQuizDiscussionUpdateByBox =
            @"select u.userName as UserName, u.userid as UserId,
    u.UserImageLarge as UserImage,
    d.Text as Text, q.id as QuizId, q.Name as QuizName, q.Url
    from zbox.quizdiscussion d 
    join Zbox.Users u on d.UserId = u.userid
	  join zbox.Quiz q on q.Id = d.QuizId
    where q.boxid = @BoxId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,d.creationTime)) >0;";
    }
}
