using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUniversityRepository : IRepository<University>
    {
        University GetUniversity(string name);
    }
}
