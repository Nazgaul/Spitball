using System;
using Zbang.Zbox.Infrastructure.Enums;
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxItemsPagedQuery : QueryBase
    {
        /// <summary>
        /// Query ctor with paging
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <param name="tabId"></param>
        public GetBoxItemsPagedQuery(long boxId, long id, Guid? tabId = null)
            : base(id)
        {
            BoxId = boxId;
            TabId = tabId;
        }

        public long BoxId { get; private set; }


        public Guid? TabId { get; set; }

    }
}
