namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class LibraryChoose
    {
        public const string GetRussianDepartments = @"select id,name,year from zbox.department
                where universityid = @universityId";


        public const string GetDepartmentsByTerm =
            @"  select l.Id as Id, l.Name as Name from zbox.maindepartment l
  where l.universityId = @universityId
  and l.name like '%'+ @term +'%'
  order by name";

        public const string GetNeedId = @"select  count(*) from zbox.student
              where UniversityId = @universityId";

        public const string GetNeedCode = @"select needcode from zbox.users where userid = @universityId";

        public const string GetUniversityDetail = @"select
                         u.Userid as Id,  
                         u.UniversityName as Name,
                         u.userimagelarge as Image
                         from zbox.users u 
                         where u.usertype = 1";

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

        public const string GetFriendsInUniversitiesByFriendsIds = 
        @"with users_cte(universityid2,facebookuserid) as (
select universityid2,facebookuserid from zbox.users where facebookuserid in @FriendsIds
),
university_cte(userid,universityname, userimage,number) as (
select top(3) u.userid, u.universityname, u.userimagelarge as userimage  , count(*) as number from zbox.users u join users_cte c on u.userid = c.universityid2
group by u.userid, u.universityname,u.userimagelarge
order by count(*) desc
)
select universityid2 as UniversityId , facebookuserid as Id  from users_cte c where universityid2 in (select userid from university_cte);";
    }
}
