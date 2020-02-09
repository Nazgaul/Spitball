using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class TutorStatsQuery : IQuery<IEnumerable<TutorStatsDto>>
    {
        public TutorStatsQuery(long userId, int days)
        {
            UserId = userId;
            Days = days;
        }
        public long UserId { get; }
        public int Days { get; }
        internal sealed class TutorStatsQueryHandler : IQueryHandler<TutorStatsQuery, IEnumerable<TutorStatsDto>>
        {
            private readonly IDapperRepository _dapperRepository;
            public TutorStatsQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorStatsDto>> GetAsync(TutorStatsQuery query, CancellationToken token)
            {
                const string sql = @"select 1 as [Period], tPrice.Price + sPrice.Price as Revenue, tPrice.Sales, Followers.Followers, 150 as Views
                                    from
                                    (
                                    select sum(price)/40 as price, count(1) as Sales
                                    from sb.[Transaction] t
                                    where [user_Id] = @UserId and [type] = 'Earned'
                                    and Created > GETUTCDATE() - @days
                                    ) tPrice
                                    ,
                                    (
                                    select sum(Price) as price
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
                                    )Followers

                                    union

                                    select 2 as [Period], tPrice.Price + sPrice.Price as Revenue, tPrice.Sales, Followers.Followers, 100 as Views
                                    from
                                    (
                                    select sum(price)/40 as price, count(1) as Sales
                                    from sb.[Transaction] t
                                    where [user_Id] = @UserId and [type] = 'Earned'
                                    and Created > (GETUTCDATE() - (@days*2)) and Created < GETUTCDATE() - @days
                                    ) tPrice
                                    ,
                                    (
                                    select sum(Price) as price
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
                                    )Followers
                                    order by [Period]";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<TutorStatsDto>(sql, new { query.Days, query.UserId });
                }
            }
        }
    }
}
