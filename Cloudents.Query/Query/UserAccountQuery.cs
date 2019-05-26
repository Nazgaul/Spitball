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

        public long Id { get; }


        internal sealed class UserAccountDataQueryHandler : IQueryHandler<UserAccountQuery, UserAccountDto>
        {
            private readonly DapperRepository _dapperRepository;

            public UserAccountDataQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            //[Cache(TimeConst.Minute * 15, "UserAccount", true)]
            public async Task<UserAccountDto> GetAsync(UserAccountQuery query, CancellationToken token)
            {
                //          const string sql = @"select u.Id, U.Balance, u.Name, u.Image, u.Email, 
                //                          case when u.UniversityId2 is null then 0 else 1 end as UniversityExists,
                //                          u.Score, u.PhoneNumberHash,
                //                          case when t.id is null then 0 else 1 end as IsTutor,
                //CASE WHEN EXISTS (
                //					 SELECT 1 FROM sb.StudyRoomUser sru WHERE sru.UserId = u.[Id] AND u.PaymentKey is null)
                //          THEN 1 
                //          ELSE 0 
                //     END AS NeedPayment
                //                      from sb.[user] u
                //                      left join sb.Tutor t
                //                       on u.Id = t.Id
                //                      where U.Id = @Id";

                const string sql = @"select u.Id, U.Balance, u.Name, u.Image, u.Email, 
                            case when u.UniversityId2 is null then 0 else 1 end as UniversityExists,
                            u.Score, u.PhoneNumberHash,
                             t.State as IsTutor
                        from sb.[user] u
                        left join sb.Tutor t
	                       on u.Id = t.Id 
                        where U.Id = @Id 
                        and (LockoutEnd is null or GetUtcDate() >= LockoutEnd);";

                const string paymentSql =
                    @"select top 1 cast(1 as bit) 
from sb.StudyRoomUser sru 
join sb.studyRoom sr on sru.StudyRoomId = sr.Id
join sb.[user] u on sru.UserId = u.Id 
join sb.Tutor t on t.Id = sr.TutorId and t.Price <> 0
where u.Id = @id and sr.TutorId <> sru.UserId
and (u.PaymentKey is null or u.PaymentKeyExpiration  < GETUTCDATE());";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    using (var grid = await conn.QueryMultipleAsync(sql + paymentSql, new { id = query.Id }))
                    {
                        var retVal = await grid.ReadSingleOrDefaultAsync<UserAccountDto>();
                        if (retVal == null)
                        {
                            return null;
                        }
                       // retVal.NeedPayment = await grid.ReadSingleOrDefaultAsync<bool>();
                        return retVal;
                    }
                }
            }
        }
    }
}