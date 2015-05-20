﻿namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class LibraryChoose
    {
        public const string GetRussianDepartments = @"select id,name,year from zbox.department
                where universityid = @universityId";


        public const string GetDepartmentsByTerm =
            @"  select l.LibraryId as Id, l.Name as Name from zbox.library l
  where l.Id = @universityId
  and l.name like '%'+ @term +'%'
  order by name";

        public const string GetNeedId = @"select  count(*) from zbox.student
              where UniversityId = @universityId";

        public const string GetNeedCode = @"select needcode from zbox.university where id = @universityId";

        public const string GetUniversityDetail = @"select
                         u.Id as Id,  
                         u.UniversityName as Name,
                         u.LargeImage as Image
                         from zbox.University u ";


        public const string GetUniversityByFriendIds = @"
select top(3) u.id as id, u.universityname as name, u.largeimage as image ,count(*)
from zbox.university u join zbox.users u2 on u.id = u2.universityid
where u2.facebookuserid in  @FriendsIds
group by u.Id, u.universityname,u.LargeImage
order by count(*) desc";

        public const string GetFriendsInUniversitiesByFriendsIds =
            @"select universityid as UniversityId , facebookuserid as Id from zbox.users where facebookuserid in @FriendsIds";

        public const string GetInitialValueOfUniversitiesBaseOnIpAddress =
   @"with country_cte(country) as (
	select country_code2 from zbox.ip_range
	where ip_from <= @IP and @IP <= ip_to
	)
select UniversityName as name, LargeImage as image, id
from zbox.university u 
left join country_cte c  on u.country = c.country
order by 
case when u.country = c.country then 0 else 1 end asc, NoOfUsers desc
offset @pageNumber*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY";
    }


   
}
