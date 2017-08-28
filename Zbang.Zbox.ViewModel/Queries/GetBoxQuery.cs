namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxQuery : IPagedQuery
    {
        public GetBoxQuery(long boxId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            BoxId = boxId;
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
        }
        public long BoxId { get; set; }

        public int PageNumber { get; }

        public int RowsPerPage { get; }
    }
}
