using Cloudents.Core.DTOs.Admin;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserDetailsQuery : IQuery<UserDetailsDto>
    {
        public AdminUserDetailsQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; }
    }
}
