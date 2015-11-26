
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxItemsPagedQuery : IPagedQuery
    {
        public GetBoxItemsPagedQuery(long boxId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
        }

        public long BoxId { get; private set; }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }


    }
}
