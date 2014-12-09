namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Email
    {
        public const string Partners = @"
--NEW USERS--
Select Count(*) from zbox.users where creationtime>getdate()-7
and universityid=@userid

--ALL USERS--
Select Count(*) from zbox.users where universityid=@userid

--NEW COURSES--
select Count(*) from zbox.box  where creationtime>getdate()-7 and University=@userid

--ALL COURSES--
select Count(*) from zbox.box  where University=@userid

--NEW ITEMS--
select Count(*) from zbox.item  where creationtime>getdate()-7
and userid in (Select userid from zbox.users where universityid=@userid)





--ALL ITEMS--
select Count(*) from zbox.item  where 
userid in (Select userid from zbox.users where universityid=@userid)

---NEW Q&A--
		select countQ+countA from (
		SELECT  (
        select Count(*) from zbox.Question  where creationtime>getdate()-7
		and userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countQ,
        (
        select Count(*) from zbox.Answer  where creationtime>getdate()-7
		and userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countA
		) t

---ALL Q&A--
		select countQ+countA from (
		SELECT  (
        select Count(*) from zbox.Question  where 
		userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countQ,
        (
        select Count(*) from zbox.Answer  where 
		userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countA
		) t
		
--National Cloudents Top 10--
select top 10 u.UniversityName as Name, (select count(*) from zbox.users where universityid = u.id) as students
from zbox.University u
where country = 'NL'
order by Students desc  ";


        public const string GetUserListByNotificationSettings =
            @"select distinct u.userid as UserId,u.email as Email, u.culture as Culture, u.UserName as UserName
      from zbox.userboxrel ub 
      join zbox.users u on ub.userid = u.userid
      join zbox.box b on ub.boxid = b.boxid
      where notificationSettings = @Notification
	  and ub.UserType in (2,3)
      and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@NotificationTime,b.updateTime)) >0;";

        public const string GetBoxPossibleUpdateByUser =
            @" select distinct b.boxid as BoxId, b.boxname as BoxName, b.PictureUrl as BoxPicture,
    (select universityname from zbox.university u where b.university = u.Id) as UniversityName, b.Url
    from zbox.userboxrel ub 
    join zbox.box b on ub.boxid = b.boxid and b.isdeleted = 0
    where
	ub.userid = @UserId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,b.updateTime)) >0;";

        public const string GetItemUpdateByBox =
            @" select u.username as UserName,u.userid as UserId, i.name as Name, i.ThumbnailUrl as Picture, i.Url
    from zbox.item i inner join zbox.users u on u.userid = i.userid
    where  i.IsDeleted = 0
    and i.boxid = @BoxId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,i.creationTime)) >0;";

        public const string GetQuizUpdateByBox = @"select u.username as UserName,u.userid as UserId, q.name as Name, 
    'http://zboxstorage.blob.core.windows.net/mailcontainer/Quiz.jpg' as Picture, q.url
	 from zbox.quiz q inner join zbox.users u on u.userid = q.userid
	 where q.boxid = @BoxId
     and publish = 1
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,q.creationTime)) > 0;";

        public const string GetQuestionUpdateByBox = @"select u.userName as UserName, u.userid as UserId, q.Text as Text
    from zbox.question q 
    join Zbox.Users u on q.UserId = u.userid
    where q.boxid = @BoxId
	and q.text is not null
    and q.IsSystemGenerated = 0
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,q.creationTime)) >0;";

        public const string GetAnswerUpdateByBox = @" select u.userName as UserName, u.userid as UserId, a.Text as Text
    from zbox.answer a 
    join Zbox.Users u on a.UserId = u.userid
    where a.boxid = @BoxId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,a.creationTime)) >0;";

        public const string GetQuizDiscussionUpdateByBox =
            @"select u.userName as UserName, u.userid as UserId, d.Text as Text, q.id as QuizId, q.Name as QuizName, q.Url
    from zbox.quizdiscussion d 
    join Zbox.Users u on d.UserId = u.userid
	  join zbox.Quiz q on q.Id = d.QuizId
    where q.boxid = @BoxId
    and DATEDIFF(MINUTE ,GETUTCDATE(),DATEADD(MINUTE,@Notification,d.creationTime)) >0;";
    }
}
