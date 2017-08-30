
using System;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxItemsPagedQuery : IPagedQuery
    {
        public GetBoxItemsPagedQuery(long boxId, Guid? tabId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            TabId = tabId;
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
        }

        public long BoxId { get; private set; }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public Guid? TabId { get; private set; }
    }
}
