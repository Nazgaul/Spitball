namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class LibraryChoose
    {
        public const string GetNeedId = @"select  top 1 UniversityId, u.MailAddress as Email, TextPopupUpper, TextPopupLower from zbox.student s inner join zbox.University u on s.UniversityId = u.Id
              where s.UniversityId = @universityId";

        public const string GetUniversityDetail = @"select
                         u.Id as Id,  
                         u.UniversityName as Name,
                         u.LargeImage as Image
                         from zbox.University u ";

        public const string GetInitialValueOfUniversitiesBaseOnIpAddress =
   @"with country_cte(country) as (
	select country_code2 from zbox.ip_range
	where ip_from <= @IP and @IP <= ip_to
	)
select UniversityName as name, LargeImage as image, id
from zbox.university u 
left join country_cte c  on u.country = c.country
where u.isdeleted = 0
order by 
case when u.country = c.country then 0 else 1 end asc, NoOfUsers desc
offset @pageNumber*@rowsperpage ROWS
FETCH NEXT @rowsperpage ROWS ONLY";
    }
}
