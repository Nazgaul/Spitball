
namespace Zbang.Zbox.ViewModel.Queries
{
    public class RecommendedCoursesQuery : IUserQuery
    {
        public RecommendedCoursesQuery(long universityId, long userId)
        {
            UserId = userId;
            UniversityId = universityId;
        }

        public long UniversityId { get; private set; }

        public long UserId { get; private set; }
    }
}
