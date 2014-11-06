using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public int GetNumberOfBoxes(University universityId)
        {
            return UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                            .Where(w => w.University == universityId)
                            .And(w => !w.IsDeleted)
                            .RowCount();
        }
    }
}
