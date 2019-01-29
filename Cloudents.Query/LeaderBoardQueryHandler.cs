
using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using AutoMapper;
using Cloudents.Infrastructure.Data;

namespace Cloudents.Query
{
    public class LeaderBoardQueryHandler : IQueryHandler<LeaderBoardQuery, LeaderBoardResultDto>
    {
        private readonly DapperRepository _dapper;
        private readonly IMapper _mapper;



        public LeaderBoardQueryHandler(DapperRepository dapper, IMapper mapper)
        {
            _dapper = dapper;
            _mapper = mapper;
        }

        public async Task<LeaderBoardResultDto> GetAsync
            (LeaderBoardQuery query, CancellationToken token)
        {
            var leaderBoardResult = await _dapper.WithConnectionAsync(async connection =>
            {
                var grid = connection.QueryMultiple(@"select top 10 Id, Name, Score, University
                                                                        from [dbo].[vwLeaderBoard]
                                                                        order by Score desc;

                                                                        select top 1 [SBLs]
                                                                        from sb.HomeStats"
                                                    );
               
                var leaderBoardDto = await grid.ReadAsync<LeaderBoardDto>();
                var stats = await grid.ReadFirstAsync<long>();
                return (stats, leaderBoardDto);
            }, token);

            var destination = new LeaderBoardResultDto();

            var orderDto = _mapper.Map<(long, IEnumerable<LeaderBoardDto>), LeaderBoardResultDto>(leaderBoardResult);
            return _mapper.Map(leaderBoardResult, destination);
        }
       
    }

}
