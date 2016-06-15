
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserConversationAndFriends : IPagedQuery
    {
        public GetUserConversationAndFriends(long userId, long universityId, string term, int pageNumber, int rowsPerPage)
        {
            UserId = userId;
            UniversityId = universityId;
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
            Term = term ?? string.Empty;
        }

        public long UserId { get; private set; }
        public long UniversityId { get; private set; }
        public string Term { get; private set; }
        public int PageNumber { get; private set; }
        public int RowsPerPage { get; private set; }
    }
}
