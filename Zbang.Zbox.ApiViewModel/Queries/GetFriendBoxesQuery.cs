
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ApiViewModel.Queries
{
    public class GetFriendBoxesQuery : GetBoxesQueryBase
    {
        public GetFriendBoxesQuery(long friendId, long userId, UserType userType)
            : base(userId, "APIGetFriendBoxes")
        {
            FriendId = friendId;
            UserType = userType;
        }
        public long FriendId { get; private set; }
        public UserType UserType { get; private set; }

        public override string QueryName
        {
            get
            {
                if (UserType == Infrastructure.Enums.UserType.Regular)
                {
                    return m_QueryName;
                }
                else
                {
                    return PageQueryPrefix + m_QueryName;
                }
            }
        }
               
    }
}
