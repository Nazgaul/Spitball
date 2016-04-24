using System;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetBoxQuestionsQuery : IPagedQuery2
    {
        public GetBoxQuestionsQuery(long boxId, int top = int.MaxValue, int skip = 0)
        {
            Top = top;
            Skip = skip;
            BoxId = boxId;
            
        }

        public static GetBoxQuestionsQuery GetBoxQueryOldVersion(long boxId, int pageNumber, int rowsPerPage)
        {
            return new GetBoxQuestionsQuery(boxId, rowsPerPage, pageNumber* rowsPerPage);
        }

        public long BoxId { get; private set; }


        public int Top { get; }

        public int Skip { get; }
    }
}
