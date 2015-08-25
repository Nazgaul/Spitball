namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxQuizesPagedQuery : IPagedQuery
    {
        public GetBoxQuizesPagedQuery(long boxId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
         
            BoxId = boxId;
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
        }

        public long BoxId { get; private set; }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

    

    }
}