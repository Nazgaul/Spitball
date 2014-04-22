using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class LibraryChoose
    {
        public const string GetDepartments = @"select id,name,year from zbox.department
                where universityid = @universityId";

        public const string GetNeedId = @"select  count(*) from zbox.student
              where UniversityId = @universityId";
    }
}
