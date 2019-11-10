using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class UserReferralsQuery : IQuery<UserReferralsDto>
    {
        public UserReferralsQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
