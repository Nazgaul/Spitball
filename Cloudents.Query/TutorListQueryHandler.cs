using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public class TutorListQueryHandler : IQueryHandler<TutorListQuery, IEnumerable<TutorListDto>>
    {
        private readonly DapperRepository _dapperRepository;

        public TutorListQueryHandler(DapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListQuery query, CancellationToken token)
        {
            var sql = @"select top 20 U.Id as UserId, U.Name, U.Image, T.Bio, T.Price, U.Score, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
                        order by Rate desc";
            using (var conn = _dapperRepository.OpenConnection())
            {
                var retVal = await conn.QueryAsync<TutorListDto>(sql, token);

                return retVal;
            }
        }
    }
}
