using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using NHibernate.Linq;

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


            //private const string Revenue = "tRevenue";
            //private const string SRevenue = "sRevenue";
            //private const string Sales = "tSales";
            //private const string SSales = "sSales";
            //private const string Followers = "followers";
            //private const string Views = "views";


            public async Task<IEnumerable<UserStatsDto>> GetAsync(UserStatsQuery query, CancellationToken token)
            {
                // UserStatsDtos(query);

                //.Where(w=>  w.Action)



                //const string tRevenueSql = @"select cast(isnull(sum(t.price), 0) as int) as price
                //                    from sb.[Transaction] t
                //                    where [user_Id] = :UserId and [action] in ('SoldDocument','ReferringUser')
                //                    and Created between :from and :to;";

                //const string sRevenueSql = @"select cast(isnull(sum(srs.Price), 0) as int) as price
                //                            from sb.StudyRoom sr
                //                            join sb.StudyRoomSession srs
                //                                on sr.Id = srs.StudyRoomId
                //                            where sr.TutorId = :UserId and srs.Ended is not null 
                //                            and srs.Created between :from and :to;";

                //const string tSalesSql = @"select count(1) as Sales
                //                    from sb.[Transaction] t
                //                    where [user_Id] = :UserId and [action] in ('SoldDocument','ReferringUser')
                //                    and Created between :from and :to;";

                //const string sSalesSql = @"select count(1) as Sales
                //                            from sb.StudyRoom sr
                //                            join sb.StudyRoomSession srs
                //                                on sr.Id = srs.StudyRoomId
                //                            where sr.TutorId = :UserId and srs.Ended is not null 
                //                            and srs.Created between :from and :to;";

                //const string followersSql = @"select count(1) as Followers
                //                            from sb.UsersRelationship
                //                            where UserId = :UserId and Created < :to;";





                //const string countrySql = @"select country from sb.[user] where Id = :UserId";



                var period = new Period(query.Days, query.UserId);

                var transactionFuture = GetTransactionPrice(period);
                var transactionPreviousFuture = GetTransactionPrice(period.PreviousPeriod());


                var studyRoomV1Future = GetStudyRoomV1Price(period);
                var studyRoomV1PreviousFuture = GetStudyRoomV1Price(period.PreviousPeriod());


                var studyRoomV2Future = GetStudyRoomV2Price(period);
                var studyRoomV2PreviousFuture = GetStudyRoomV2Price(period.PreviousPeriod());

                var followersFuture = GetFollowers(period);
                var followersPreviousFuture = GetFollowers(period.PreviousPeriod());


                var viewsFuture = CreateViews(period);
                var viewsPreviousFuture = CreateViews(period.PreviousPeriod());
                //     //var sqlQueries = new Dictionary<string, string>
                //     //{
                //     //    { Revenue, tRevenueSql },
                //     //    { SRevenue, sRevenueSql },
                //     //    { Sales, tSalesSql },
                //     //    { SSales, sSalesSql },
                //     //    { Followers, followersSql },
                //     //    { Views, viewsSql }
                //     //};

                //     var futureQueries = new Dictionary<string, (IFutureValue<int> current, IFutureValue<int> previous)>();

                //foreach (var sql in sqlQueries)
                //{
                //    if (sql.Key == Followers)
                //    {
                //        futureQueries.Add(sql.Key, CreateSqlFollowersQuery(sql.Value, period));
                //    }
                //    else
                //    {
                //        futureQueries.Add(sql.Key, CreateSqlQuery(sql.Value, period));
                //    }
                //}

                var countryFuture = _session.Query<User>()
                    .Where(w => w.Id == query.UserId)
                    .Select(s => s.SbCountry)
                    .ToFutureValue();

                //var resDictionary = new Dictionary<string, (int current, int previous)>();

                //foreach (var item in futureQueries)
                //{
                //    var currentRes = await item.Value.current.GetValueAsync(token);
                //    var previousRes = await item.Value.previous.GetValueAsync(token);
                //    resDictionary.Add(item.Key, (currentRes, previousRes));
                //}

                var country = await countryFuture.GetValueAsync(token);

                var currentResult = new UserStatsDto();
                var previousResult = new UserStatsDto();

                currentResult.Revenue = country.ConversationRate * transactionFuture.Value.Item1.GetValueOrDefault() +
                                        studyRoomV1Future.Value.Item1.GetValueOrDefault() + studyRoomV2Future.Value.Item1.GetValueOrDefault();
                currentResult.Sales = transactionFuture.Value.Item2 + studyRoomV1Future.Value.Item2 + studyRoomV2Future.Value.Item2;
                currentResult.Followers = followersFuture.Value;
                currentResult.Views = viewsFuture.Value;

                previousResult.Revenue = country.ConversationRate * transactionPreviousFuture.Value.Item1.GetValueOrDefault() +
                                         studyRoomV1PreviousFuture.Value.Item1.GetValueOrDefault() + studyRoomV2PreviousFuture.Value.Item1.GetValueOrDefault();
                previousResult.Sales = transactionPreviousFuture.Value.Item2 + studyRoomV1PreviousFuture.Value.Item2 + studyRoomV2PreviousFuture.Value.Item2;
                previousResult.Followers = followersPreviousFuture.Value;
                previousResult.Views = viewsPreviousFuture.Value;

                return new List<UserStatsDto>()
                {
                    currentResult,
                    previousResult
                };
            }

            private IFutureValue<Tuple<decimal?, int>> GetTransactionPrice(Period query)
            {
                return _session.Query<Transaction>().Where(w => new[]
                    {
                        TransactionActionType.SoldDocument,
                        TransactionActionType.ReferringUser
                    }.Contains(w.Action) && w.User.Id == query.UserId)
                    .Where(w => w.Created >= query.From && w.Created <= query.To)
                    .GroupBy(g => 1)
                    .Select(s => Tuple.Create(s.Sum(s2 => (decimal?)s2.Price), s.Count())
                    ).ToFutureValue();
            }

            private IFutureValue<Tuple<decimal?, int>> GetStudyRoomV1Price(Period query)
            {
                return _session.Query<StudyRoomSession>()
                      .Where(w => w.StudyRoomVersion.GetValueOrDefault() == 0)
                      .Where(w => w.StudyRoom.Tutor.Id == query.UserId && w.Ended != null)
                      .Where(w => w.Created >= query.From && w.Created <= query.To)
                      .GroupBy(g => 1)
                      .Select(s => Tuple.Create(s.Sum(s2 => (decimal?)s2.Price.GetValueOrDefault()), s.Count()))
                    .ToFutureValue();
            }

            private IFutureValue<Tuple<decimal?, int>> GetStudyRoomV2Price(Period query)
            {
                return _session.Query<StudyRoomSessionUser>()
                    .Where(w => w.StudyRoomSession.StudyRoom.Tutor.Id == query.UserId )
                    .Where(w=>w.Receipt != null)
                    .Where(w => w.StudyRoomSession.Created >= query.From && w.StudyRoomSession.Created <= query.To)
                    .GroupBy(g => 1)
                    .Select(s => Tuple.Create(s.Sum(s2 => (decimal?)s2.TotalPrice), s.Count()))
                    .ToFutureValue();
            }

            private IFutureValue<int> GetFollowers(Period query)
            {
                /*  const string followersSql = @"select count(1) as Followers
                                            from sb.UsersRelationship
                                            where UserId = :UserId and Created < :to;";*/
                return _session.Query<Follow>()
                     .Where(w => w.Followed.Id == query.UserId)
                     .Where(w => w.Created >= query.From && w.Created <= query.To)
                     .GroupBy(g => 1)
                     .Select(s => s.Count())
                     .ToFutureValue();
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

                public DateTime To { get; }

                public Period PreviousPeriod()
                {
                    return new Period(this);
                }
            }

            //private (IFutureValue<int> current, IFutureValue<int> previous) CreateSqlQuery(string sql, Period query)
            //{
            //    var x = CreateSqlQuery2(sql, query);
            //    var y = CreateSqlQuery2(sql, query.PreviousPeriod());

            //    return (x, y);
            //}

            private IFutureValue<int> CreateViews(Period query)
            {
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

                return _session.CreateSQLQuery(viewsSql)
                                   .SetInt64("UserId", query.UserId)
                                   .SetDateTime("from", query.From)
                                   .SetDateTime("to", query.To)
                                   .FutureValue<int>();
            }

            //private (IFutureValue<int> current, IFutureValue<int> previous) CreateSqlFollowersQuery(string sql, Period query)
            //{
            //    var x = CreateSqlFollowersQuery2(sql, query);
            //    var y = CreateSqlFollowersQuery2(sql, query.PreviousPeriod());

            //    return (x, y);
            //}

            //private IFutureValue<int> CreateSqlFollowersQuery2(string sql, Period query)
            //{
            //    return _session.CreateSQLQuery(sql)
            //                          .SetInt64("UserId", query.UserId)
            //                          .SetDateTime("to", query.To)
            //                          .FutureValue<int>();
            //}
        }
    }
}
