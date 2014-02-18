using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Admin
    {
        public const string UsersInUniversity = @"select u.UserId as Id,u.UserImage as Image,u.username as Name,d.name  + ' ' + d.year as Department ,u.CreationTime as JoinDate 
from zbox.Users u left join zbox.department d on u.department = d.id 
where u.UniversityId2 = @universityId;";
    }
}
