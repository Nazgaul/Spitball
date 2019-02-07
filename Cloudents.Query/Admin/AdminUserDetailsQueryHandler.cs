﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserDetailsQueryHandler : IQueryHandler<AdminUserDetailsQuery, UserDetailsDto>
    {
      
        private readonly IConfigurationKeys _provider;


        public AdminUserDetailsQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
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
			isnull((select min(Created) from sb.[Transaction] where [Action] = 'SignUp' and User_id = U.Id),cast(0 as datetime)) as Joined,
			U.PhoneNumberConfirmed
                        from sb.[User] U
                        left join sb.University Un
	                        on U.UniversityId2 = Un.Id
                        where U.Id = @Id
            group by U.Id, U.Name, Email, PhoneNumberHash, Un.Name, U.Country, U.Score, U.FraudScore, U.Balance, 
            case when U.LockOutEnd is null or U.LockOutEnd < getutcdate() then 1
	            else 0 end,
			case when U.LockoutEnd is null then 0
			else 1 end,
			U.PhoneNumberConfirmed";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                return await connection.QuerySingleOrDefaultAsync<UserDetailsDto>(sql,
                    new
                    {
                        id = query.UserId,
                    });
            }

                    
         

         

        }
    }
}
