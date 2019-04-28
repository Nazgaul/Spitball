using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using Dapper;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserAccountDataQueryHandler : IQueryHandler<UserDataByIdQuery, UserAccountDto>
    {
        private readonly DapperRepository _dapperRepository;

        public UserAccountDataQueryHandler(DapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        //[Cache(TimeConst.Minute * 15, "UserAccount", true)]
        public async Task<UserAccountDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            const string sql = @"select u.Id, U.Balance, u.Name, u.Image, u.Email, 
                            case when u.UniversityId2 is null then 0 else 1 end as UniversityExists,
                            u.Score, u.PhoneNumberHash,
                            case when t.id is null then 0 else 1 end as IsTutor,
	 CASE WHEN EXISTS (
							 SELECT 1 FROM sb.StudyRoomUser sru WHERE sru.UserId = u.[Id] AND u.PaymentKey is null)
            THEN 1 
            ELSE 0 
       END AS NeedPayment
                        from sb.[user] u
                        left join sb.Tutor t
	                        on u.Id = t.Id
                        where U.Id = @Id";
            using (var conn = _dapperRepository.OpenConnection())
            {
                var retVal = await conn.QueryFirstAsync<UserAccountDto>(sql, new { id = query.Id });
                return retVal;
            }
        }
    }
}