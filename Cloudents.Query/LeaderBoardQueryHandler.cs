using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Cloudents.Query
{
    public class LeaderBoardQueryHandler : IQueryHandler<LeaderBoardQuery, LeaderBoardResultDto>
    {
        private readonly DapperRepository _dapper;

        public LeaderBoardQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<LeaderBoardResultDto> GetAsync
            (LeaderBoardQuery query, CancellationToken token)
        {
            return await _dapper.WithConnectionAsync(async connection =>
            {
                var grid = connection.QueryMultiple(@" select top 10 U.Id, U.Name, Score, Un.Name as University, u.Image
    from sb.[user] U
    join sb.University UN
        on U.UniversityId2 = UN.Id
    where LockoutEnd is null
	and PhoneNumberConfirmed = 1 and EmailConfirmed = 1
	order by Score desc;

                                                                        select top 1 [SBLs]
                                                                        from sb.HomeStats");
               
                var leaderBoardDto = await grid.ReadAsync<LeaderBoardDto>();
                var stats = await grid.ReadFirstAsync<long>();
                return new LeaderBoardResultDto()
                {
                    LeaderBoard = leaderBoardDto,
                    Points = stats
                };
            }, token);
           
        }
       
    }

}
