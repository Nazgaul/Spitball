namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetBoxQuestionsQuery : IPagedQuery
    {
        public GetBoxQuestionsQuery(long boxId, int pageNumber, int rowsPerPage)

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
