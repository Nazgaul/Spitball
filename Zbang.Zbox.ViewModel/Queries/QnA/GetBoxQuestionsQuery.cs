using System;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetBoxQuestionsQuery : IPagedQuery
    {
        public GetBoxQuestionsQuery(long boxId, DateTime timeStamp, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
            TimeStamp = timeStamp;
        }

        public long BoxId { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public int PageNumber { get; }

        public int RowsPerPage { get; }
    }
}
