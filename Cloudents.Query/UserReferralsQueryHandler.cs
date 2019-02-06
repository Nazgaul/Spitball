using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;
namespace Cloudents.Query.Query
{
    public class UserReferralsQueryHandler : IQueryHandler<UserReferralsQuery, UserReferralsDto>
    {
        private readonly DapperRepository _dapper;



        public UserReferralsQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        public async Task<UserReferralsDto> GetAsync(UserReferralsQuery query, CancellationToken token)
        {
            return await _dapper.WithConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<UserReferralsDto>(@"select count(1) as Referrals
                    from sb.[Transaction] where [Action] = 'ReferringUser' and User_Id = @Id"
                   ,new { Id = query.Id });
            }, token);
        }
    }
}
