using System;
using Zbang.Zbox.Infrastructure.Enums;
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxItemsPagedQuery : QueryPagedBase
    {
        /// <summary>
        /// Query ctor without paging
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="Id"></param>
        public GetBoxItemsPagedQuery(long boxId, long id)
            : this(boxId, id, 0, OrderBy.LastModified, null)
        {
        }

        /// <summary>
        /// Query ctor with paging
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="Id"></param>
        /// <param name="pageNumber"></param>
        public GetBoxItemsPagedQuery(long boxId, long id, int pageNumber, OrderBy order, Guid? tabId)
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
