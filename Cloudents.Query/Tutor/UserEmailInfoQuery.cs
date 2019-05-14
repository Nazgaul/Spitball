using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class UserEmailInfoQuery: IQuery<UserEmailInfoDto>
    {
        public UserEmailInfoQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }
        internal sealed class UserEmailInfoQueryHandler : IQueryHandler<UserEmailInfoQuery, UserEmailInfoDto>
        {
            private readonly DapperRepository _dapperRepository;
            public UserEmailInfoQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }
            public async Task<UserEmailInfoDto> GetAsync(UserEmailInfoQuery query, CancellationToken token)
            {
                using (var conn = _dapperRepository.OpenConnection())
                {
                    const string sql = @"select Email, u.Name, un.Name as University, u.Country, 
                                            u.PhoneNumberHash as PhoneNumber
                                        from sb.[User] u
                                        left join sb.University un
	                                        on u.UniversityId2 = un.Id
                                        where u.Id = @UserId";
                    return await conn.QueryFirstOrDefaultAsync<UserEmailInfoDto>(sql, new { query.UserId });
                }
            }
        }
    }
}
