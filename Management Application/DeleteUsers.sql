DECLARE @Userid bigint,@ASPid uniqueidentifier, @SysUserid bigint

Set @Userid = (select userid from zbox.users where email=@Email)
Set @ASPid = (SELECT [MembershipUserId] FROM zbox.users where email=@Email)
Set @SysUserid=22886
 BEGIN TRANSACTION
GO
-- Items
update [Zbox].item set userid=@SysUserid, isdeleted=1,isdirty=1 where userid=@userid 
update [Zbox].item set userid=@SysUserid, isdeleted=1,isdirty=1 where [QuestionId] in (Select [QuestionId] from Zbox.Question where userid=@Userid)
update [Zbox].item set userid=@SysUserid, isdeleted=1,isdirty=1 where AnswerId in (Select AnswerId  from zbox.Answer where userid=@Userid)



-- Change Box (Only private) ownership to userid=22886 
update [Zbox].box set ownerid=@SysUserid, isdeleted=1,isdirty=1 where [OwnerId]=@userid and Discriminator is NULL and [MembersCount]<=1
update [Zbox].box set ownerid=@SysUserid, isdirty=1 where [OwnerId]=@userid and Discriminator is NULL and [MembersCount]>1
update [Zbox].box set ownerid=@SysUserid, isdirty=1 where [OwnerId]=@userid and Discriminator=2 


-- Question and Answer 
update zbox.Answer   set userid=@SysUserid, isdeleted=1 where [QuestionId] in (Select [QuestionId] from Zbox.Question where userid=@userid)
update Zbox.Answer   set userid=@SysUserid, isdeleted=1 where userid=@userid
update Zbox.Question set userid=@SysUserid, isdeleted=1 where userid=@userid


--Comment and Replies 
update [Zbox].[CommentReplies] set [AuthorId]=@SysUserid where [AuthorId]= @userid


-- libraray 

update zbox.Library set userid=@SysUserid where UserId=@Userid

-- Membership tables
delete from [dbo].[AspNetRoles]      where Id=@ASPid
delete from [dbo].[AspNetUserLogins] where userId=@ASPid
delete from [dbo].[AspNetUserRoles]  where userid=@ASPid
delete from [dbo].[AspNetUsers]	   where Id=@ASPid

-- Item Comments and Replies 
 
 delete from [Zbox].[ItemCommentReply] where itemid in (select itemid from zbox.ItemComment where userid=@userid) 
 delete from [Zbox].[ItemComment] where userid=@userid 
 delete from [Zbox].[ItemCommentReply] where userid=@userid 
 


--updates
 delete from [Zbox].[NewUpdates] where [UserId]=@userid
 delete from [Zbox].[NewUpdates] where [QuestionId] in (select  questionid from Zbox.Question where userid=@userid)
 delete from [Zbox].[NewUpdates] where [AnswerId]	in (Select answerid from zbox.Answer where [QuestionId] in (Select [QuestionId] from Zbox.Question where userid=@userid))
 delete from [Zbox].[NewUpdates] where [Answerid]	in (select answerid from Zbox.Answer where userid=@userid)
 delete from [Zbox].[NewUpdates] where [ItemId] in (select itemid from Zbox.item where userid=@userid)
 delete from [Zbox].[NewUpdates] where [QuizId] in ( select  quizid from Zbox.Quiz where userid=@userid) 



--Invite 
delete from zbox.invite where senderid= @userid 
delete from zbox.invite where [UserBoxRelId] in (Select [UserBoxRelId] from Zbox.userBoxrel where userid=@userid)

-- Boxes that follow 
Delete from Zbox.userBoxrel where userid=@userid


-- Quizzes 
 update [Zbox].quiz set userid=@SysUserid, isdeleted=1,isdirty=1 where userid=@userid 
 delete from [Zbox].[QuizDiscussion] where userid=@userid
 delete from [Zbox].[SolvedQuestion] where userid=@userid
 delete from [Zbox].[SolvedQuiz] where userid=@userid
 
-- Reputation 
 Delete from [Zbox].[Reputation] where userid=@userid

-- Messages 
 delete from [Zbox].[Message] where senderid=@userid
 delete from [Zbox].[Message] where [RecepientId]=@userid

-- Rate 
 delete  from [Zbox].[ItemRate] where [OwnerId]=@userid
 
--  UserLibraryRel
 
delete from Zbox.UserLibraryRel where [UserId]=@userid
 
-- Zbox.CommentLike
 
delete from Zbox.CommentLike where [OwnerId]=@userid
 
-- Zbox.ReplyLike
 
delete from Zbox.ReplyLike where [OwnerId]=@userid
-- user table
update zbox.users set UniversityId2=null where UniversityId2=@Userid
delete from zbox.users where userid=@userid 
COMMIT TRANSACTION
