DECLARE @Userid bigint,@ASPid uniqueidentifier, @SysUserid bigint, @chatRoom uniqueidentifier;

Set @Userid = (select userid from zbox.users where email=@Email)
Set @ASPid = (SELECT [MembershipUserId] FROM zbox.users where email=@Email)
Set @SysUserid=22886
 BEGIN TRANSACTION
GO
-- Items
update [Zbox].item set userid=@SysUserid, isdeleted=1,isdirty=1 where userid=@userid 
update [Zbox].item set userid=@SysUserid, isdeleted=1,isdirty=1 where [QuestionId] in (Select [QuestionId] from Zbox.Question where userid=@Userid)
update [Zbox].item set userid=@SysUserid, isdeleted=1,isdirty=1 where AnswerId in (Select AnswerId  from zbox.Answer where userid=@Userid)

--remove signalr
delete from zbox.SignalRConnection where userid = @userid

-- Change Box (Only private) ownership to userid=22886 
update [Zbox].box set ownerid=@SysUserid, isdeleted=1,isdirty=1 where [OwnerId]=@userid and Discriminator is NULL and [MembersCount]<=1
update [Zbox].box set ownerid=@SysUserid, isdirty=1 where [OwnerId]=@userid and Discriminator is NULL and [MembersCount]>1
update [Zbox].box set ownerid=@SysUserid, isdirty=1 where [OwnerId]=@userid and Discriminator=2 


-- Question and Answer 
update zbox.Answer   set userid=@SysUserid, isdeleted=1 where [QuestionId] in (Select [QuestionId] from Zbox.Question where userid=@userid)
update Zbox.Answer   set userid=@SysUserid, isdeleted=1 where userid=@userid
update Zbox.Question set userid=@SysUserid, isdeleted=1 where userid=@userid





-- libraray 

update zbox.Library set userid=@SysUserid where UserId=@Userid

-- Membership tables
delete from [dbo].[AspNetRoles]      where Id=@ASPid
delete from [dbo].[AspNetUserLogins] where userId=@ASPid
delete from [dbo].[AspNetUserRoles]  where userid=@ASPid
delete from [dbo].[AspNetUsers]	   where Id=@ASPid


--updates
 delete from [Zbox].[NewUpdates] where [UserId]=@userid
 delete from [Zbox].[NewUpdates] where [QuestionId] in (select  questionid from Zbox.Question where userid=@userid)
 delete from [Zbox].[NewUpdates] where [AnswerId]	in (Select answerid from zbox.Answer where [QuestionId] in (Select [QuestionId] from Zbox.Question where userid=@userid))
 delete from [Zbox].[NewUpdates] where [Answerid]	in (select answerid from Zbox.Answer where userid=@userid)
 delete from [Zbox].[NewUpdates] where [ItemId] in (select itemid from Zbox.item where userid=@userid)
 delete from [Zbox].[NewUpdates] where [QuizId] in ( select  quizid from Zbox.Quiz where userid=@userid) 
 delete from [Zbox].[NewUpdates] where [ItemCommentId] in ( select  ItemCommentId from  [Zbox].[ItemComment] where userid=@userid) 


-- Item Comments and Replies 
 
 delete from [Zbox].[ItemCommentReply] where itemid in (select itemid from zbox.ItemComment where userid=@userid) 
 delete from [Zbox].[ItemComment] where userid=@userid 
 delete from [Zbox].[ItemCommentReply] where userid=@userid 
 

 delete from zbox.badge where userid = @userid;




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

 -- flashcard 
 update [Zbox].Flashcard set userid=@SysUserid, isdeleted=1,isdirty=1 where userid=@userid 
 delete from [Zbox].FlashcardLike where userid=@userid
 delete from [Zbox].FlashcardPin where userid=@userid
 delete from [Zbox].[FlashcardSolve] where userid=@userid
 
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

delete from zbox.UserLocationActivity where UserId = @userid

-- chat
DECLARE vend_cursor CURSOR  
    FOR select ChatRoomId from zbox.chatuser where userid = @userid  
OPEN vend_cursor  
FETCH NEXT FROM vend_cursor into @chatRoom;  

  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	 PRINT @chatRoom 
	 delete from zbox.ChatMessage where ChatRoomId = @chatRoom;
	 delete from zbox.ChatUser where ChatRoomId = @chatRoom;
	 delete from zbox.ChatRoom where Id = @chatRoom;
	 FETCH NEXT FROM vend_cursor into @chatRoom;  
END
CLOSE vend_cursor;  
DEALLOCATE vend_cursor;

-- user table
delete from zbox.users where userid=@userid ;

COMMIT TRANSACTION
