
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxWithUserQuery : IUserQuery, IPagedQuery
    {
        public GetBoxWithUserQuery(long boxId, long userId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            BoxId = boxId;
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
            UserId = userId;


        }
        public long BoxId { get; set; }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

        public long UserId { get; private set; }
    }
}
