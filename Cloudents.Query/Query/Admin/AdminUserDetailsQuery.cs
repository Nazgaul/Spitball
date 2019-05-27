﻿using Cloudents.Core.DTOs.Admin;
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

            private readonly DapperRepository _dapper;


            public AdminUserDetailsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<UserDetailsDto> GetAsync(AdminUserDetailsQuery query, CancellationToken token)
            {
                var sql = @"select U.Id, U.Name, Email, PhoneNumberHash as PhoneNumber, Un.Name as University, U.Country, U.Score, U.FraudScore, 
		(select count(1) from sb.[Transaction] T where  U.Id = T.[User_id] and T.[Action] = 'ReferringUser')  as ReferredCount,
		U.Balance, 
	        case when U.LockOutEnd is null or U.LockOutEnd < getutcdate() then 1
	        else 0 end as IsActive,
			case when U.LockoutEnd is null then 0
			else 1 end as WasSuspended,
			(select min(Created) from sb.[Transaction] where [Action] = 'SignUp' and User_id = U.Id) as Joined,
			U.PhoneNumberConfirmed,
			U.EmailConfirmed,
            U.LastOnline,
			case when T.id is not null then T.State else 'Not a Tutor' end as IsTutor
                        from sb.[User] U
                        left join sb.University Un
	                        on U.UniversityId2 = Un.Id
						left join sb.Tutor T
							on U.Id = T.Id
                        where U.Id = @Id or U.Email = @Email or U.PhoneNumberHash = @Email
                            or U.PhoneNumberHash = CONCAT('+972',right(@Email,9))
            group by U.Id, U.Name, Email, PhoneNumberHash, Un.Name, U.Country, U.Score, U.FraudScore, U.Balance, 
            case when U.LockOutEnd is null or U.LockOutEnd < getutcdate() then 1
	            else 0 end,
			case when U.LockoutEnd is null then 0
			else 1 end,
			U.PhoneNumberConfirmed,
			U.EmailConfirmed, U.LastOnline,
            case when T.id is not null then T.State else 'Not a Tutor' end";

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
