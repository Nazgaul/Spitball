
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

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

        public long UserId { get; private set; }
    }
}
