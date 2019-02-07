using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
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
                   , new { query.Id });
            }, token);
        }
    }
}
