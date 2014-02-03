using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public class GetUserWithFriendQuery : QueryBase
    {
        public GetUserWithFriendQuery(long userId, long friendId)
            :base(userId)
        {
            FriendId = friendId;
        }
        public long FriendId { get; private set; }
    }
}
