using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Admin
    {
        public const string UsersInUniversity = @"select u.UserId as Id,u.UserImage as Image,u.username as Name,'x' as Department ,u.CreationTime as JoinDate from zbox.Users u where u.UniversityId2 = @universityId;";
    }
}
