using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain.DataAccess
{
    interface IDepartmentRepository : Zbang.Zbox.Infrastructure.Repositories.IRepository<Department>
    {
        int GetBoxesInDepartment(Department dep);
    }
}
