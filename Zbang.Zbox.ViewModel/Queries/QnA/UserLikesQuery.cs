namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class UserLikesQuery
    {
        public UserLikesQuery(long userId, long boxId)
        {
            UserId = userId;
            BoxId = boxId;
        }

        public long UserId { get; private set; }
        public long BoxId { get; private set; }
    }
}
