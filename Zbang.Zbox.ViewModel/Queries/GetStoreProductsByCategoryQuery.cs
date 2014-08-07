
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetStoreProductsByCategoryQuery
    {
        public GetStoreProductsByCategoryQuery(int? categoryId, int? universityId)
        {
            CategoryId = categoryId;
            UniversityId = universityId;
        }

        public int? CategoryId { get; private set; }

        public int? UniversityId { get; private set; }
    }
}
