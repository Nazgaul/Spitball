using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
   public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public int GetNumberOfBoxes(University universityId)
        {
            //this cant be done with query over
            return UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                            .Where(w => w.University == universityId)
                            .RowCount();
                            //.Select(Projections.Sum<Department>(s => s.NoOfBoxes)).SingleOrDefault<int>();
            // return query.SingleOrDefault<University>();
        }
    }
}
