using Cloudents.Core.DTOs.Admin;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserInfoQuery : IQuery<UserInfoDto>
    {
        public AdminUserInfoQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; }
    }
}
