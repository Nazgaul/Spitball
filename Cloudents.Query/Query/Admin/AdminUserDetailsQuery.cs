using System;
using System.Linq;
using Cloudents.Core.DTOs.Admin;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserDetailsQuery : IQueryAdmin<UserDetailsDto>
    {
        public AdminUserDetailsQuery(string userId, string country)
        {
            UserId = userId;
            Country = country;
        }

        private string UserId { get; }
        public string Country { get; }

        internal sealed class AdminUserDetailsQueryHandler : IQueryHandler<AdminUserDetailsQuery, UserDetailsDto>
        {

            private readonly IStatelessSession _session;

            public AdminUserDetailsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<UserDetailsDto> GetAsync(AdminUserDetailsQuery query, CancellationToken token)
            {

                long.TryParse(query.UserId, out var tmpId);


                var dbQuery = _session.Query<User>()
                    .Fetch(f => f.University)
                    .Fetch(f => f.Tutor)
                    .Where(w => w.Id == tmpId || w.Email == query.UserId || w.PhoneNumber == query.UserId);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    dbQuery = dbQuery.Where(w => w.Country == query.Country);
                }

                return await dbQuery.Select(s => new UserDetailsDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    University = s.University.Name,
                    Country = s.Country,
                    ReferredCount = _session.Query<ReferUserTransaction>().Count(w => w.User.Id == s.Id),
                    Balance = s.Transactions.Balance,
                    IsActive = s.LockoutEnd == null || s.LockoutEnd < DateTime.UtcNow,
                    WasSuspended = s.LockoutEnd != null,
                    Joined = _session.Query<AwardMoneyTransaction>()
                        .Where(w => w.Action == TransactionActionType.SignUp && w.User.Id == s.Id).Select(s2 => s2.Created).FirstOrDefault()
                        ,
                    PhoneNumberConfirmed = s.PhoneNumberConfirmed,
                    EmailConfirmed = s.EmailConfirmed,
                    LastOnline = s.LastOnline,
                    LockoutReason = s.LockoutReason,
                    TutorState = s.Tutor.State,
                    PaymentExists = s.PaymentExists == PaymentStatus.Done,
                    TutorPrice = s.Tutor.Price.SubsidizedPrice ?? s.Tutor.Price.Price,
                    CalendarExists = _session.Query<GoogleTokens>().Any(w => w.Id == s.Id.ToString())
                }).SingleOrDefaultAsync(token);
                //                string sql = @"select U.Id, U.Name, Email, PhoneNumberHash as PhoneNumber, Un.Name as University, U.Country, U.Score, 
                //		(select count(1) from sb.[Transaction] T where  U.Id = T.[User_id] and T.[Action] = 'ReferringUser')  as ReferredCount,
                //		U.Balance, 
                //	        case when U.LockOutEnd is null or U.LockOutEnd < getUtcDate() then 1
                //	        else 0 end as IsActive,
                //			case when U.LockoutEnd is null then 0
                //			else 1 end as WasSuspended,
                //			(select min(Created) from sb.[Transaction] where [Action] = 'SignUp' and User_id = U.Id) as Joined,
                //			U.PhoneNumberConfirmed,
                //			U.EmailConfirmed,
                //            U.LastOnline,
                //            U.LockoutReason,
                //t.state as TutorState,
                //case when U.PaymentExists is null or U.PaymentExists = 0 then 0 else 1 end as PaymentExists

                //                        from sb.[User] U
                //                        left join sb.University Un
                //	                        on U.UniversityId2 = Un.Id
                //						left join sb.Tutor T
                //							on U.Id = T.Id
                //                        where (U.Id = @Id or U.Email = @Email or U.PhoneNumberHash = @Email)";

                //                if (!string.IsNullOrEmpty(query.Country))
                //                {
                //                    sql += " and u.Country = @Country;";
                //                }
                //                using (var connection = _dapper.OpenConnection())
                //                {

                //                    return await connection.QuerySingleOrDefaultAsync<UserDetailsDto>(sql,
                //                        new
                //                        {
                //                            id = tmpId,
                //                            email = query.UserId,
                //                            country = query.Country
                //                        });
                //                }
                //}
            }
        }
    }
}
