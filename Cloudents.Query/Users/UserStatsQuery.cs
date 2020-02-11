using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserStatsQuery : IQuery<IEnumerable<UserStatsDto>>
    {
        public UserStatsQuery(long userId, int days)
        {
            UserId = userId;
            Days = days;
        }
        public long UserId { get; }
        public int Days { get; }


        internal sealed class UserStatsQueryHandler : IQueryHandler<UserStatsQuery, IEnumerable<UserStatsDto>>
        {
            //private readonly IStatelessSession _session;
            private readonly IDapperRepository _dapperRepository;
            public UserStatsQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<UserStatsDto>> GetAsync(UserStatsQuery query, CancellationToken token)
            {

                //const string tRevenueSQL = @"select sum(price) as price
                //                    from sb.[Transaction] t
                //                    where [user_Id] = :UserId and [type] = 'Earned'
                //                    and Created between GETUTCDATE() - :startDays and GETUTCDATE() - :endDays;";

                //const string sRevenueSQL = @"select sum(Price) as price
                //                            from sb.StudyRoom sr
                //                            join sb.StudyRoomSession srs
                //                             on sr.Id = srs.StudyRoomId
                //                            where sr.TutorId = :UserId and srs.Ended is not null 
                //                            and srs.Created between GETUTCDATE() - :startDays and GETUTCDATE() - :endDays;";

                //const string salesSQL = @"select count(1) as Sales
                //                    from sb.[Transaction] t
                //                    where [user_Id] = :UserId and [type] = 'Earned'
                //                    and Created between GETUTCDATE() - :startDays and GETUTCDATE() - :endDays;";

                //const string followersSQL = @"select count(1) as Followers
                //            from sb.UsersRelationship
                //            where UserId = :UserId and Created < GETUTCDATE() - :endDays;";


                //const string viewsSQL = @"select sum([Views]) as [Views] from sb.Document where [State] = 'ok' and UserId = :UserId;";

                //const string viewsHistorySQL = @"with cte as (
                //                            select d.Id, max(dh.[Views]) as [Views]
                //                            from [sb].[DocumentHistory] dh
                //                            join sb.Document d
                //                             on d.Id = dh.Id and d.State = 'ok'
                //                            where d.UserId = :UserId
                //                            and dh.EndDate < GETUTCDATE() - :startDays
                //                            group by d.Id
                //                            )
                //                            select sum([Views]) as [Views] from cte;";
                //var period = new Period(query.Days);


                //var sqlQueries = new Dictionary<string, >

                //CreateSqlQuery()

                //var tRevenueFirstFuture = _session.CreateSQLQuery(tRevenueSQL)
                //                .SetInt64("UserId", query.UserId)
                //                .SetInt32("startDays", query.Days)
                //                .SetInt32("endDays", 0)
                //                .FutureValue<int>();

                //var sRevenueFirstFuture = _session.CreateSQLQuery(sRevenueSQL)
                //                .SetInt64("UserId", query.UserId)
                //                .SetInt32("startDays", query.Days)
                //                .SetInt32("endDays", 0)
                //                .FutureValue<int>();

                //var thisPeriod = { 0, 7 };
                //var lastPeriod = { 7, 14 };




                //var dictionary = new Dictionary<string, string>();
                //dictionary.Add("F)


                const string sql = @"with cteLast as (
select d.Id, max(dh.[Views]) as [MaxViews], min(dh.[Views]) as [MinViews]
from [sb].[DocumentHistory] dh
join sb.Document d
    on d.Id = dh.Id and d.State = 'ok'
where d.UserId = @UserId
and dh.EndDate < GETUTCDATE() - @days and dh.BeginDate > GETUTCDATE() - @days*2
group by d.Id
),
cteThis as (
 select d.Id, max(dh.[Views]) as [MaxViews], min(dh.[Views]) as [MinViews]
                                from [sb].[DocumentHistory] dh
                                join sb.Document d
                                 on d.Id = dh.Id and d.State = 'ok'
                                where d.UserId = @UserId
                                and dh.BeginDate > GETUTCDATE() - @days
                                group by d.Id
)

     select 1 as [Period], tPrice.Price as tRevenue, 
								sPrice.Price as sRevenue,
								tPrice.Sales + sPrice.Sales as Sales, Followers.Followers, [Views].[Views] as Views
                                from
                                (
                                select sum(price) as price, count(1) as Sales
                                from sb.[Transaction] t
                                where [user_Id] = @UserId and [action] in ('SoldDocument','ReferringUser')
                                and Created > GETUTCDATE() - @days
                                ) tPrice
                                ,
                                (
                                select sum(Price) as price, count(1) as Sales
                                from sb.StudyRoom sr
                                join sb.StudyRoomSession srs
                                 on sr.Id = srs.StudyRoomId
                                where sr.TutorId = @UserId and srs.Ended is not null and srs.Created > GETUTCDATE() - @days
                                )
                                sPrice
                                ,
                                (
                                select count(1) as Followers
                                from sb.UsersRelationship
                                where UserId = @UserId
                                )Followers,
                                (
                               select sum([MaxViews]) - sum(MinViews) as [Views] from cteThis
                                )[Views]

                                union all

                                select 2 as [Period], tPrice.Price as tRevenue, 
								sPrice.Price as sRevenue,
								tPrice.Sales + sPrice.Sales as Sales, Followers.Followers, [Views].[Views] as Views
                                from
                                (
                                select sum(price) as price, count(1) as Sales
                                from sb.[Transaction] t
                                where [user_Id] = @UserId and [action] in ('SoldDocument','ReferringUser')
                                and Created > (GETUTCDATE() - (@days*2)) and Created < GETUTCDATE() - @days
                                ) tPrice
                                ,
                                (
                                select sum(Price) as price, count(1) as Sales
                                from sb.StudyRoom sr
                                join sb.StudyRoomSession srs
                                 on sr.Id = srs.StudyRoomId
                                where sr.TutorId = @UserId and srs.Ended is not null 
                                and srs.Created > GETUTCDATE() - (@days*2)
                                and srs.Created < GETUTCDATE() - @days
                                )
                                sPrice
                                ,
                                (
                                select count(1) as Followers
                                from sb.UsersRelationship
                                where UserId = @UserId and (Created < GETUTCDATE() - @days or Created is null)
                                )Followers,
                                (
                                select sum([MaxViews]) - sum(MinViews) as [Views] from cteLast
                                ) [Views]
                                order by [Period];

                        select country from sb.[user] where Id = @UserId";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    using (var multi = conn.QueryMultiple(sql, new { query.Days, query.UserId }))
                    {
                        var resObj = await multi.ReadAsync<TempUserStats>();
                        var countryRes = await multi.ReadFirstAsync<string>();
                        Country country = countryRes;
                        return resObj.Select(s => new UserStatsDto() {
                            Followers = s.Followers,
                            Views = s.Views,
                            Sales = s.Sales,
                            Revenue = country.ConversationRate * s.tRevenue + s.sRevenue
                        });
                    }
                }
            }

            private class TempUserStats
            {
                public decimal tRevenue { get; set; }
                public decimal sRevenue { get; set; }
                public int Sales { get; set; }
                public int Views { get; set; }
                public int Followers { get; set; }
            }

            //private class Period
            //{
            //    public Period(int days)
            //    {
            //        From = DateTime.UtcNow;
            //        To = DateTime.UtcNow.AddDays(-days);
            //        Days = days;
            //    }

            //    private Period(Period currentPeriod)
            //    {
            //        From = currentPeriod.To;
            //        To = From.AddDays(-Days);
            //    }

            //    private int Days { get; }

            //    public DateTime From { get; }

            //    public DateTime To { get; set; }

            //    public Period NextPeriod()
            //    {
            //        return new Period(this);
            //    }
            //}

            //private (IFutureValue<int> current, IFutureValue<int> previous) CreateSqlQuery(string sql, Period query)
            //{
            //    var x = CreateSqlQuery2(sql, query);
            //    var y = CreateSqlQuery2(sql, query.NextPeriod());

            //    return (x, y);
            //}

            //private IFutureValue<int>  CreateSqlQuery2(string sql, Period query)
            //{

            // return   _session.CreateSQLQuery(sql)
            //                    .SetInt64("UserId", query.UserId)
            //                    .SetInt32("startDays", query.Days)
            //                    .SetInt32("endDays", 0)
            //                    .FutureValue<int>();
            //}
        }
    }
}
