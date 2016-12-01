using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ReputationRepository : IReputationRepository
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
select count(*)*50 as questionScore from zbox.Question a join zbox.Box b on a.BoxId = b.BoxId and b.Discriminator = 2 where a.UserId = @userid and issystemgenerated = 0
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
    }

    public interface IReputationRepository 
    {
        int GetUserReputation(long userId);
    }
}
