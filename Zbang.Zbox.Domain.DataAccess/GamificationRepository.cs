using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class GamificationRepository : NHibernateRepository<Badge> ,IGamificationRepository
    {
        public int GetUserReputation(long userId)
        {
            var sqlQuery = UnitOfWork.CurrentSession.CreateSQLQuery(@"declare @userid bigint = :userId;
with quiz as 
(
select count(*)*50 as quizScore from zbox.Quiz q join zbox.Box b on q.BoxId = b.BoxId and b.Discriminator = 2 where q.UserId = @userid and q.Publish = 1 and q.IsDeleted = 0
),
 item as 
(
select count(*)*100 as itemScore from zbox.item i join zbox.Box b on i.BoxId = b.BoxId and b.Discriminator = 2 where i.UserId=@userid and i.IsDeleted = 0
),
answers as
(
select count(*)*100 as answerScore from zbox.Answer a join zbox.Box b on a.BoxId = b.BoxId and b.Discriminator = 2 where a.UserId = @userid
),
question as
(
select count(*)*50 as questionScore from zbox.Question a join zbox.Box b on a.BoxId = b.BoxId and b.Discriminator = 2 where a.UserId = @userid and isSystemGenerated = 0
),
rate as 
(
select count(*)*150 as rateScore from zbox.ItemRate ir join zbox.Item i  on ir.ItemId = i.ItemId join zbox.Box b on i.BoxId = b.BoxId and b.Discriminator = 2
where i.UserId = @userid
),
invite as (
select coalesce( sum(invitesScore),0) as inviteScore from (
select 
case
when TypeOfMsg = 3 then 150
when TypeOfMsg = 2 then 50
else 0
end as invitesScore
 from zbox.Invite
where isused = 1 and senderid = @userid) t
),
share as (
select count(distinct(DATEADD(DAY, DATEDIFF(DAY, 0, r.creationTime), 0)))*50 as shareScore from zbox.Reputation r where r.Action = 7 and r.UserId = @userid )

select 500 + quizScore + itemScore + answerScore + questionScore + rateScore + inviteScore + shareScore 
from quiz,item,answers,question,rate,invite,share
");
            sqlQuery.SetInt64("userId", userId);
            var retVal = sqlQuery.UniqueResult<int>();
            return retVal;
        }

        public int CalculateReputation(long userId)
        {
            var sqlQuery = UnitOfWork.CurrentSession.CreateSQLQuery(@"declare @userid bigint = :userId, 
@itemLikePoints int = :itemLikePoints,
@itemView int = :itemView, 
@itemDownload int = :itemDownload, 
@quizView int = :quizView, 
@quizSolve int = :quizSolve, 
@quizLike int = :quizLike ,
@flashcardView int = :flashcardView, 
@flashcardLike int = :flashcardLike, 
@commentLike int = :commentLike, 
@replyLike int = :replyLike,
@badge1 int = :badge1, @badge2 int = :badge2, @badge3 int = :badge3, @badge4 int = :badge4, @badge5 int  = :badge5 ;
with itemScore as 
(
	select coalesce(sum(i.LikeCount),0)*@itemLikePoints + coalesce(sum(i.NumberOfViews),0) * @itemView + coalesce(sum(i.NumberOfDownloads),0) * @itemDownload as score  from zbox.item i where i.UserId  = @userid and isdeleted = 0
),
quizScore as 
(
	select coalesce(sum(q.LikeCount),0)*@quizLike +  coalesce(sum(q.NumberOfViews),0)*@quizView + coalesce(sum(q.SolveCount),0) * @quizSolve  as score from zbox.Quiz q where q.Publish = 1 and q.IsDeleted = 0 and q.UserId = @userid
),
flashcardScore as 
(
	select coalesce(sum(f.NumberOfViews),0)*@flashcardView + coalesce(sum(f.LikeCount),0) * @flashcardLike as score from zbox.Flashcard f where f.IsDeleted = 0 and f.UserId = @userid
),
commentScore as (
	select coalesce(sum(q.likecount),0)*@commentLike as  score from Zbox.Question q where q.UserId = @userid
),
replyScore as (
	select coalesce(sum(a.likecount),0)*@replyLike as score from Zbox.Answer a where a.UserId = @userid
),
badges as (
select coalesce(sum(x),0)+ @badge1 as score from (
select 
	case name
		when 2 then @badge2
		when 3 then @badge3
		when 4 then @badge4
		when 5 then @badge5 
		end as x
 from zbox.Badge
where userid = @userid
and Progress = 100) t
)
select i.score + q.score +f.score + c.score + r.score + b.score
 from itemScore i ,quizScore q,flashcardScore f,commentScore c,replyScore r, badges b
");
            sqlQuery.SetInt64("userId", userId);
            sqlQuery.SetInt32("itemLikePoints", ReputationConst.ItemLike);
            sqlQuery.SetInt32("itemView", ReputationConst.ItemView);
            sqlQuery.SetInt32("itemDownload", ReputationConst.ItemDownload);
            sqlQuery.SetInt32("quizView", ReputationConst.QuizView);
            sqlQuery.SetInt32("quizSolve", ReputationConst.QuizSolve);
            sqlQuery.SetInt32("quizLike", ReputationConst.QuizLike);
            sqlQuery.SetInt32("flashcardView", ReputationConst.FlashcardView);
            sqlQuery.SetInt32("flashcardLike", ReputationConst.FlashcardLike);
            sqlQuery.SetInt32("commentLike", ReputationConst.CommentLike);
            sqlQuery.SetInt32("replyLike", ReputationConst.ReplyLike);
            sqlQuery.SetInt32("badge1", ReputationConst.BadgeRegister);
            sqlQuery.SetInt32("badge2", ReputationConst.BadgeFollowClass);
            sqlQuery.SetInt32("badge3", ReputationConst.BadgeCreateQuizzes);
            sqlQuery.SetInt32("badge4", ReputationConst.BadgeUploadFiles);
            sqlQuery.SetInt32("badge5", ReputationConst.BadgeLikes);

            var retVal = sqlQuery.UniqueResult<int>();
            return retVal;
        }


        public Badge GetBadgeOfUser(long userId, BadgeType type)
        {
            var query = UnitOfWork.CurrentSession.QueryOver<Badge>();
            query.Where(b => b.User.Id == userId);
            query.Where(b => b.Name == type);
            return query.SingleOrDefault<Badge>();
        }


    }
}
