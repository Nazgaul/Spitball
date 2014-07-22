

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetStoreProductQuery
    {
        public GetStoreProductQuery(long productId)
        {
            ProductId = productId;
        }

        public long ProductId { get; private set; }
    }
}
