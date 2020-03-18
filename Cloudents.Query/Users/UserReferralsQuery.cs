using Cloudents.Core.DTOs.Users;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserReferralsQuery : IQuery<UserReferralsDto>
    {
        public UserReferralsQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }

        internal sealed class UserReferralsQueryHandler : IQueryHandler<UserReferralsQuery, UserReferralsDto>
        {
            private readonly IDapperRepository _dapper;



            public UserReferralsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }
            public async Task<UserReferralsDto> GetAsync(UserReferralsQuery query, CancellationToken token)
            {
                using (var connection = _dapper.OpenConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<UserReferralsDto>(@"select count(1) as Referrals
                    from sb.[Transaction] where [Action] = 'ReferringUser' and User_Id = @Id"
                        , new { query.Id });
                }


            }
        }
    }
}
