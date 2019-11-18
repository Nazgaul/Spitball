using Cloudents.Core.DTOs;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

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
                u.Country,
                          t.State as IsTutor,
                            coalesce(
                                iif(u.PaymentExists != 0 , 0, null),
								iif(u.Country != 'IL', 0 , null),
                                1
                            )as NeedPayment
                      from sb.[user] u
                      left join sb.Tutor t
                     on u.Id = t.Id 
                      where U.Id = @Id
                      and (LockoutEnd is null or GetUtcDate() >= LockoutEnd);";


                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QuerySingleOrDefaultAsync<UserAccountDto>(sql, new { id = query.Id });

                }
            }
        }
    }
}