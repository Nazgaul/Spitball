
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetStoreProductsByCategoryQuery
    {
        public GetStoreProductsByCategoryQuery(int? categoryId)
        {
            CategoryId = categoryId;
        }

        public int? CategoryId { get; private set; }
    }
}
