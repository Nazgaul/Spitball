--update user balance
update sb.[user] 
set balance = (Select sum(price) from sb.[Transaction] where User_id = sb.[User].id and type != 'stake')
where balance != (Select sum(price) from sb.[Transaction] where User_id = sb.[User].id and type != 'stake')

--update user score
update top(1000) sb.[user]
set score = (select sum(price) from sb.[Transaction] where Type = 'Earned' and price > 0 and User_id = sb.[User].id)
where score != (select sum(price) from sb.[Transaction] where Type = 'Earned' and price > 0 and User_id = sb.[User].id)

update top (10000) sb.[user] 
set balance = 0
where not exists (Select top 1 * from sb.[Transaction] where User_id = sb.[User].id)
and balance <>0
and Fictive = 0


update top (10000) sb.[user] 
set score = 0
where not exists (Select top 1 * from sb.[Transaction] where User_id = sb.[User].id)
and score <>0
and Fictive = 0


--delete user
begin tran
declare @UserId bigint = 160052
delete from sb.[Transaction] where user_id =  @UserId
update sb.question
set CorrectAnswer_id = null
where id in (select questionid from sb.Answer where userid =  @UserId)
delete from sb.[Transaction] where answerid in (select id from
sb.Answer where userid =  @UserId)
delete from sb.Answer where userid =  @UserId
update sb.question set CorrectAnswer_id = null where userid =  @UserId
delete from sb.[Transaction] where answerid in (select id from
sb.answer where questionid in (select id from sb.question where userid
=  @UserId))
delete from sb.answer where questionid in (select id from sb.question
where userid =  @UserId)
delete from sb.question where userid =  @UserId
delete from sb.DocumentsTags where [DocumentId] in (select Id from sb.Document where UserId = @UserId)
delete from sb.Document where UserId = @UserId
delete from sb.UsersTags where UserId = @UserId
delete from sb.UserLocation where UserId = @UserId
delete from sb.UserLogin where UserId = @UserId
delete from sb.UsersCourses where UserId = @UserId
delete from sb.[user] where id =  @UserId
rollback
commit