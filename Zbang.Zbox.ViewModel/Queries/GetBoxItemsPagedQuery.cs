
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxItemsPagedQuery : QueryBase
    {
        /// <summary>
        /// Query ctor with paging
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="id"></param>
        public GetBoxItemsPagedQuery(long boxId, long id)
            : base(id)
        {
            BoxId = boxId;
        }

        public long BoxId { get; private set; }



    }
}
