
namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public class GetBoxesQuery : IUserQuery, IPagedQuery
    {
        public GetBoxesQuery(long userId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            UserId = userId;
        }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public long UserId { get; }
    }
}
