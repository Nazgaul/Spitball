
namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetDepartmentsByTermQuery
    {
        public GetDepartmentsByTermQuery(long universityId, string term)
        {
            Term = term;
            UniversityId = universityId;
        }

        public string Term { get; private set; }
        public long UniversityId { get; private set; }
    }
}
