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
