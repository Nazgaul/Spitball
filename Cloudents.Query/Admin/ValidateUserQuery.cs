using Cloudents.Core.Entities;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class ValidateUserQuery : IQuery<UserRolesDto?>
    {
        public ValidateUserQuery(string email)
        {
            Email = email;
        }


        private string Email { get; }



        internal sealed class CoursesByTermQueryHandler : IQueryHandler<ValidateUserQuery, UserRolesDto?>
        {
            private readonly QuerySession _session;

            public CoursesByTermQueryHandler(QuerySession session)
            {
                _session = session;
            }

            public async Task<UserRolesDto?> GetAsync(ValidateUserQuery query, CancellationToken token)
            {
                var result = await _session.StatelessSession.Query<AdminUser>()
                    .WithOptions(w => w.SetComment(nameof(ValidateUserQuery)))
                      .Where(w => w.Email == query.Email)
                      .SingleOrDefaultAsync(token);
                if (result == null)
                {
                    return null;
                }
                return new UserRolesDto()
                {
                    Country = result.Country,
                    SbCountry = result.SbCountry,
                    Id = result.Id
                };
            }
        }
    }

    public class UserRolesDto
    {
        public Guid Id { get; set; }
        public string? Country { get; set; }

        public Country? SbCountry { get; set; }
    }
}
