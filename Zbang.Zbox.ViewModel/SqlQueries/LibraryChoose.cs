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
                         u.UniversityName as Name
                         u.userimagelarge as Image,
                         (select count(*) from zbox.users where universityid2 = u.userid) as MemberCount
                         from zbox.users u 
                         where u.usertype = 1 
                         and u.userid = @UserId";

        public const string GetUniversityByFriendIds = @"
with users_cte(username,userimage,universityid2) as (
select username,userimage,universityid2 from zbox.users where facebookuserid in @FriendsIds
),
university_cte(userid,universityname, userimage,number) as (
select top(3) u.userid, u.universityname,u.userimagelarge as userimage  , count(*) as number from zbox.users u join users_cte c on u.userid = c.universityid2
group by u.userid, u.universityname,u.userimagelarge
order by count(*) desc
)
select userid as id,universityname as name, userimage as image  from university_cte u;";

        public const string GetFriendsInUniversitiesByFriendsIds = @"
with users_cte(username,userimage,universityid2) as (
select username,userimage,universityid2 from zbox.users where facebookuserid in @FriendsIds
),
university_cte(userid,universityname, userimage,number) as (
select top(3) u.userid, u.universityname, u.userimagelarge as userimage  , count(*) as number from zbox.users u join users_cte c on u.userid = c.universityid2
group by u.userid, u.universityname,u.userimagelarge
order by count(*) desc
)
select username as Name,userimage as Image,universityid2 as UniversityId  from users_cte c where universityid2 in (select userid from university_cte);";
    }
}
