using Cloudents.Core.Entities;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class ValidateUserQuery : IQuery<UserRolesDto>
    {
        public ValidateUserQuery(string email)
        {
            Email = email;
        }


        private string Email { get; }



        public sealed class CoursesByTermQueryHandler : IQueryHandler<ValidateUserQuery, UserRolesDto>
        {
            private readonly QuerySession _session;

            public CoursesByTermQueryHandler(QuerySession session)
            {
                _session = session;
            }

            public async Task<UserRolesDto> GetAsync(ValidateUserQuery query, CancellationToken token)
            {
                var result = await _session.StatelessSession.Query<AdminUser>()
                      .Where(w => w.Email == query.Email)
                      //.Fetch(f => f.Roles)
                      .SingleOrDefaultAsync(token);
                if (result == null)
                {
                    return null;
                }
                return new UserRolesDto()
                {
                    //Roles = result.Roles.Select(s => s.Role),
                    Country = result.Country,
                    Id = result.Id
                };
            }
        }
    }

    public class UserRolesDto
    {
        // public IEnumerable<string> Roles { get; set; }
        public Guid Id { get; set; }
        public string Country { get; set; }
    }
}
