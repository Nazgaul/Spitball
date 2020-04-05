using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using NHibernate;
using System;
using System.Collections.Generic;
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

        private long UserId { get; }
        private int Days { get; }


        internal sealed class UserStatsQueryHandler : IQueryHandler<UserStatsQuery, IEnumerable<UserStatsDto>>
        {
            private readonly IStatelessSession _session;
            public UserStatsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }


            private const string TRevenue = "tRevenue";
            private const string SRevenue = "sRevenue";
            private const string TSales = "tSales";
            private const string SSales = "sSales";
            private const string Followers = "followers";
            private const string Views = "views";


            public async Task<IEnumerable<UserStatsDto>> GetAsync(UserStatsQuery query, CancellationToken token)
            {


                const string tRevenueSql = @"select cast(isnull(sum(price), 0) as int) as price
                                    from sb.[Transaction] t
                                    where [user_Id] = :UserId and [action] in ('SoldDocument','ReferringUser')
                                    and Created between :from and :to;";

                const string sRevenueSql = @"select cast(isnull(sum(Price), 0) as int) as price
                                            from sb.StudyRoom sr
                                            join sb.StudyRoomSession srs
                                                on sr.Id = srs.StudyRoomId
                                            where sr.TutorId = :UserId and srs.Ended is not null 
                                            and srs.Created between :from and :to;";

                const string tSalesSql = @"select count(1) as Sales
                                    from sb.[Transaction] t
                                    where [user_Id] = :UserId and [action] in ('SoldDocument','ReferringUser')
                                    and Created between :from and :to;";

                const string sSalesSql = @"select count(1) as Sales
                                            from sb.StudyRoom sr
                                            join sb.StudyRoomSession srs
                                                on sr.Id = srs.StudyRoomId
                                            where sr.TutorId = :UserId and srs.Ended is not null 
                                            and srs.Created between :from and :to;";

                const string followersSql = @"select count(1) as Followers
                                            from sb.UsersRelationship
                                            where UserId = :UserId and Created < :to;";




                const string viewsSql = @"with cte as (
                                            select d.Id, max(dh.[Views]) as [MaxViews], min(dh.[Views]) as [MinViews]
                                            from [sb].[DocumentHistory] dh
                                            join sb.Document d
                                                on d.Id = dh.Id and d.State = 'ok'
                                            where d.UserId = :UserId
                                            and dh.EndDate < :to and dh.BeginDate > :from
                                            group by d.Id
                                            )
                                            select cast(isnull(sum([MaxViews]), 0) - isnull(sum(MinViews), 0) as int) as [Views] from cte;";

                const string countrySql = @"select country from sb.[user] where Id = :UserId";

                var period = new Period(query.Days, query.UserId);


                var sqlQueries = new Dictionary<string, string>
                {
                    { TRevenue, tRevenueSql },
                    { SRevenue, sRevenueSql },
                    { TSales, tSalesSql },
                    { SSales, sSalesSql },
                    { Followers, followersSql },
                    { Views, viewsSql }
                };

                var futureQueries = new Dictionary<string, (IFutureValue<int> current, IFutureValue<int> previous)>();

                foreach (var sql in sqlQueries)
                {
                    if (sql.Key == Followers)
                    {
                        futureQueries.Add(sql.Key, CreateSqlFollowersQuery(sql.Value, period));
                    }
                    else
                    {
                        futureQueries.Add(sql.Key, CreateSqlQuery(sql.Value, period));
                    }
                }
                var countryFuture = _session.CreateSQLQuery(countrySql)
                    .SetInt64("UserId", query.UserId)
                    .FutureValue<string>();
                var resDictionary = new Dictionary<string, (int current, int previous)>();

                foreach (var item in futureQueries)
                {
                    var currentRes = await item.Value.current.GetValueAsync(token);
                    var previousRes = await item.Value.previous.GetValueAsync(token);
                    resDictionary.Add(item.Key, (currentRes, previousRes));
                }

                var countryRes = await countryFuture.GetValueAsync(token);
                Country country = countryRes;

                var currentResult = new UserStatsDto();
                var previousResult = new UserStatsDto();

                currentResult.Revenue = country.ConversationRate * resDictionary[TRevenue].current + resDictionary[SRevenue].current;
                currentResult.Sales = resDictionary[TSales].current + resDictionary[SSales].current;
                currentResult.Followers = resDictionary[Followers].current;
                currentResult.Views = resDictionary[Views].current;

                previousResult.Revenue = country.ConversationRate * resDictionary["tRevenue"].previous + resDictionary["sRevenue"].previous;
                previousResult.Sales = resDictionary[TSales].previous + resDictionary[SSales].previous;
                previousResult.Followers = resDictionary[Followers].previous;
                previousResult.Views = resDictionary[Views].previous;

                return new List<UserStatsDto>()
                {
                    currentResult,
                    previousResult
                };
            }


            private class Period
            {
                public Period(int days, long userId)
                {
                    From = DateTime.UtcNow.AddDays(-days);
                    To = DateTime.UtcNow;
                    Days = days;
                    UserId = userId;
                }

                private Period(Period currentPeriod)
                {
                    To = currentPeriod.From;
                    From = To.AddDays(-currentPeriod.Days);
                    UserId = currentPeriod.UserId;
                }

                private int Days { get; }
                public long UserId { get; }
                public DateTime From { get; }

                public DateTime To { get; set; }

                public Period NextPeriod()
                {
                    return new Period(this);
                }
            }

            private (IFutureValue<int> current, IFutureValue<int> previous) CreateSqlQuery(string sql, Period query)
            {
                var x = CreateSqlQuery2(sql, query);
                var y = CreateSqlQuery2(sql, query.NextPeriod());

                return (x, y);
            }

            private IFutureValue<int> CreateSqlQuery2(string sql, Period query)
            {

                return _session.CreateSQLQuery(sql)
                                   .SetInt64("UserId", query.UserId)
                                   .SetDateTime("from", query.From)
                                   .SetDateTime("to", query.To)
                                   .FutureValue<int>();
            }

            private (IFutureValue<int> current, IFutureValue<int> previous) CreateSqlFollowersQuery(string sql, Period query)
            {
                var x = CreateSqlFollowersQuery2(sql, query);
                var y = CreateSqlFollowersQuery2(sql, query.NextPeriod());

                return (x, y);
            }

            private IFutureValue<int> CreateSqlFollowersQuery2(string sql, Period query)
            {
                return _session.CreateSQLQuery(sql)
                                      .SetInt64("UserId", query.UserId)
                                      .SetDateTime("to", query.To)
                                      .FutureValue<int>();
            }
        }
    }
}
