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

        public const string GetUniversityDetail = @"select  
                         
                         u.userimage as Image,
                         (select count(*) from zbox.users where universityid2 = u.userid) as MemberCount
                         from zbox.users u 
                         where u.usertype = 1 
                         and u.userid = @UserId";
    }
}
