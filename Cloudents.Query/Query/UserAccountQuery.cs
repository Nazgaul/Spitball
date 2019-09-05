using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Query
{
    public class UserAccountQuery : IQuery<UserAccountDto>
    {
        public UserAccountQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }


        internal sealed class UserAccountDataQueryHandler : IQueryHandler<UserAccountQuery, UserAccountDto>
        {
            private readonly IDapperRepository _dapperRepository;

            public UserAccountDataQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<UserAccountDto> GetAsync(UserAccountQuery query, CancellationToken token)
            {
                //TODO: to nhibernate
                const string sql = @"select u.Id, U.Balance, u.Name, u.Image, u.Email, 
                          case when u.UniversityId2 is null then 0 else 1 end as UniversityExists,
                          u.Score, u.PhoneNumberHash,
                          t.State as IsTutor,
                            coalesce(
                                iif(u.Country != 'IL', 0 , null),
                                iif(u.PaymentExists != 0 and u.PaymentKeyExpiration > GetUtcDate(), 0, null),
                                1
                            )as NeedPayment
                      from sb.[user] u
                      left join sb.Tutor t
                     on u.Id = t.Id 
                      where U.Id = @Id
                      and (LockoutEnd is null or GetUtcDate() >= LockoutEnd);";


                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QuerySingleOrDefaultAsync<UserAccountDto>(sql, new {id = query.Id});
                    
                }
            }
        }
    }
}