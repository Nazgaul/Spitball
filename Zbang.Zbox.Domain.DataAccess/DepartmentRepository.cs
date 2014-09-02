using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class DepartmentRepository : NHibernateRepository<Department>, IDepartmentRepository
    {
        public int GetBoxesInDepartment(Department dep)
        {
            return UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                               .Where(w => w.Department == dep)
                               .And(w => w.IsDeleted == false)
                               .RowCount();
        }
    }
}
