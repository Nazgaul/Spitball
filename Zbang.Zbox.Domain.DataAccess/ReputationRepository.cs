using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ReputationRepository : NHibernateRepository<Reputation>, IReputationRepository
    {
        public int GetUserReputation(long userId)
        {
            var sqlQuery = UnitOfWork.CurrentSession.CreateSQLQuery(@"declare @userid int = :userId;
with quiz as 
(
select count(*)*300 as quizScore from zbox.Quiz q where q.UserId = @userid and q.Publish = 1
),
 item as 
(
select count(*)*100 as itemScore from zbox.item i where i.UserId=@userid and i.IsDeleted = 0
),
answers as
(
select count(*)*100 as answerScore from zbox.Answer a where a.UserId = @userid
),
question as
(
select count(*)*50 as questionScore from zbox.Question a where a.UserId = @userid
),
rate as 
(
select coalesce(sum(rate),0) as rateScore from (
select 
case 
when ir.Rate = 3 then 50
when ir.rate = 4 then 100
when ir.rate = 5 then 150
else 0
end as rate
 from zbox.ItemRate ir join zbox.Item i on ir.ItemId = i.ItemId
where i.UserId = @userid) t
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
where isused = 1 and senderid = 1) t
),
share as (
select count(*)*50 as shareScore from zbox.Reputation r where r.Action = 7 and r.UserId = @userid )
,
itemComment as
(
select count(*)*30 as itemCommentScore from zbox.ItemComment a where a.UserId = @userid
),
itemReply as
(
select count(*)*15 as itemReplyScore from zbox.ItemCommentReply a where a.UserId = @userid
)
select 500 + quizScore + itemScore + answerScore + questionScore + rateScore + inviteScore + shareScore + itemCommentScore + itemReplyScore
from quiz,item,answers,question,rate,invite,share,itemComment,itemReply
");
            sqlQuery.SetInt64("userId", userId);
            var retVal = sqlQuery.UniqueResult<int>();
            return retVal;
        }
    }

    public interface IReputationRepository : IRepository<Reputation>
    {
        int GetUserReputation(long userId);
    }
}
