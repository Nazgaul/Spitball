using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUniversityRepository : IRepository<University>
    {
        int GetNumberOfBoxes(University universityId);
        //int GetNumberOfUsers(long universityId);
        //int GetNumberOfQuizzes(long universityId);
        //int GetNumberOfItems(long universityId);

        UniversityStats GetStats(long universityId);
        int GetAdminScore(long universityId);
    }
}
