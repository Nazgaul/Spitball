--update user balance
update sb.[user] 
set balance = (Select sum(price) from sb.[Transaction] where User_id = sb.[User].id)
where balance != (Select sum(price) from sb.[Transaction] where User_id = sb.[User].id)

--delete user
declare @UserId bigint = 1001
delete from sb.[Transaction] where user_id =  @UserId
update sb.question
set CorrectAnswer_id = null
where id in (select questionid from sb.Answer where userid =  @UserId)
delete from sb.[Transaction] where answerid in (select id from sb.Answer where userid =  @UserId)
delete from sb.Answer where userid =  @UserId
update sb.question set CorrectAnswer_id = null where userid =  @UserId
delete from sb.[Transaction] where answerid in (select id from sb.answer where questionid in (select id from sb.question where userid =  @UserId))
delete from sb.answer where questionid in (select id from sb.question where userid =  @UserId)
delete from sb.question where userid =  @UserId
delete from sb.[user] where id =  @UserId