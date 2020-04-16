using System;
using System.Linq;
using Cloudents.Core.DTOs.Admin;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class UserDetailsQuery : IQueryAdmin<UserDetailsDto>
    {
        public UserDetailsQuery(string userId, string country)
        {
            UserId = userId;
            Country = country;
        }

        private string UserId { get; }
        public string Country { get; }

        internal sealed class UserDetailsQueryHandler : IQueryHandler<UserDetailsQuery, UserDetailsDto>
        {

            private readonly IStatelessSession _session;

            public UserDetailsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<UserDetailsDto> GetAsync(UserDetailsQuery query, CancellationToken token)
            {

                long.TryParse(query.UserId, out var tmpId);


                var dbQuery = _session.Query<User>()
                    .WithOptions(w => w.SetComment(nameof(UserDetailsQuery)))
                    //.Fetch(f => f.University)
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
                   // University = s.University.Name,
                    Country = s.Country,
                    ReferredCount = _session.Query<ReferUserTransaction>().Count(w => w.User.Id == s.Id),
                    Balance = s.Transactions.Balance,
                    IsActive = s.LockoutEnd == null || s.LockoutEnd < DateTime.UtcNow,
                    WasSuspended = s.LockoutEnd != null,
                    Joined = s.FinishRegistrationDate,
                    PhoneNumberConfirmed = s.PhoneNumberConfirmed,
                    EmailConfirmed = s.EmailConfirmed,
                    LastOnline = s.LastOnline,
                    LockoutReason = s.LockoutReason,
                    TutorState = s.Tutor.State,
                    PaymentExists = s.PaymentExists == PaymentStatus.Done,
                    TutorPrice = s.Tutor.Price.SubsidizedPrice ?? s.Tutor.Price.Price,
                    CalendarExists = _session.Query<GoogleTokens>().Any(w => w.Id == s.Id.ToString()),
                    UserType = s.UserType2
                }).SingleOrDefaultAsync(token);
               
            }
        }
    }
}
