using System;
using Zbang.Zbox.Infrastructure.Enums;
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxItemsPagedQuery : QueryPagedBase
    {
        /// <summary>
        /// Query ctor with paging
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="id"></param>
        /// <param name="pageNumber"></param>
        /// <param name="order"></param>
        /// <param name="tabId"></param>
        public GetBoxItemsPagedQuery(long boxId, long id, int pageNumber = 0, OrderBy order = OrderBy.LastModified, Guid? tabId = null)
            : base(id, pageNumber)
        {
            BoxId = boxId;
            Order = order;
            TabId = tabId;
        }

        public long BoxId { get; private set; }

        public OrderBy Order { get; private set; }

        public Guid? TabId { get; set; }

    }
}
