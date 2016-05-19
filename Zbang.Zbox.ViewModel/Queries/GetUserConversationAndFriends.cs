
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserConversationAndFriends
    {
        public GetUserConversationAndFriends(long userId, long universityId, string term)
        {
            UserId = userId;
            UniversityId = universityId;
            Term = term ?? string.Empty;
        }

        public long UserId { get; private set; }
        public long UniversityId { get; private set; }
        public string Term { get; private set; }
    }
}
