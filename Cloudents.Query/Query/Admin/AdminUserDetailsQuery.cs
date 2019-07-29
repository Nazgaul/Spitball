using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserDetailsQuery : IQuery<UserDetailsDto>
    {
        public AdminUserDetailsQuery(string userId)
        {
            UserId = userId;
        }
        private string UserId { get; }

        internal sealed class AdminUserDetailsQueryHandler : IQueryHandler<AdminUserDetailsQuery, UserDetailsDto>
        {

            private readonly IDapperRepository _dapper;


            public AdminUserDetailsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<UserDetailsDto> GetAsync(AdminUserDetailsQuery query, CancellationToken token)
            {
                const string sql = @"select U.Id, U.Name, Email, PhoneNumberHash as PhoneNumber, Un.Name as University, U.Country, U.Score, U.FraudScore, 
		(select count(1) from sb.[Transaction] T where  U.Id = T.[User_id] and T.[Action] = 'ReferringUser')  as ReferredCount,
		U.Balance, 
	        case when U.LockOutEnd is null or U.LockOutEnd < getUtcDate() then 1
	        else 0 end as IsActive,
			case when U.LockoutEnd is null then 0
			else 1 end as WasSuspended,
			(select min(Created) from sb.[Transaction] where [Action] = 'SignUp' and User_id = U.Id) as Joined,
			U.PhoneNumberConfirmed,
			U.EmailConfirmed,
            U.LastOnline,
            U.LockoutReason,
t.state as TutorState
			
                        from sb.[User] U
                        left join sb.University Un
	                        on U.UniversityId2 = Un.Id
						left join sb.Tutor T
							on U.Id = T.Id
                        where U.Id = @Id or U.Email = @Email or U.PhoneNumberHash = @Email;";

                using (var connection = _dapper.OpenConnection())
                {
                    long.TryParse(query.UserId, out var tmpId);
                    return await connection.QuerySingleOrDefaultAsync<UserDetailsDto>(sql,
                        new
                        {
                            id = tmpId,
                            email = query.UserId
                        });
                }
            }
        }
    }
}
