using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using AutoMapper;
using Cloudents.Infrastructure.Data;

namespace Cloudents.Query
{
    public class LeaderBoardQueryHandler : IQueryHandler<LeaderBoardQuery, LeaderBoardQueryResult>
    {
        private readonly DapperRepository _dapper;
        private readonly IMapper _mapper;



        public LeaderBoardQueryHandler(DapperRepository dapper, IMapper mapper)
        {
            _dapper = dapper;
            _mapper = mapper;
        }

        public async Task<LeaderBoardQueryResult> GetAsync
            (LeaderBoardQuery query, CancellationToken token)
        {
            var leaderBoardResult = await _dapper.WithConnectionAsync(async connection =>
            {
                var grid = connection.QueryMultiple(@"select top 10 U.Name, Score, Un.Name as University
                                                                        from sb.[user] U
                                                                        join sb.University UN

                                                                            on U.UniversityId2 = UN.Id
                                                                        where LockoutEnd is null
                                                                        order by Score desc;
                                                                        select top 1 [SBLs]
                                                                        from sb.HomeStats"
                                                    );
               
                var leaderBoardDto = grid.Read<LeaderBoardDto>();
                var stats = grid.ReadFirst<long>();
                return (stats, leaderBoardDto);
            }, token);

            var destination = new LeaderBoardQueryResult();

            var orderDto = _mapper.Map<(long, IEnumerable<LeaderBoardDto>), LeaderBoardQueryResult>(leaderBoardResult);
            return _mapper.Map(leaderBoardResult, destination);
        }
       
    }

    public class LeaderBoardQueryResult
    {
        public long SBL { get; set; }
        public IEnumerable<LeaderBoardDto> LeaderBoard { get; set; }
    }
}
