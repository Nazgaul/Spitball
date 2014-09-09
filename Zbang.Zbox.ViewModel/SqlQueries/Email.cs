namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Email
    {
        public const string Partners = @"
--NEW USERS--
Select Count(*) from zbox.users where creationtime>getdate()-7
and universityid=@userid

--ALL USERS--
Select Count(*) from zbox.users where universityid=@userid

--NEW COURSES--
select Count(*) from zbox.box  where creationtime>getdate()-7 and University=@userid

--ALL COURSES--
select Count(*) from zbox.box  where University=@userid

--NEW ITEMS--
select Count(*) from zbox.item  where creationtime>getdate()-7
and userid in (Select userid from zbox.users where universityid=@userid)





--ALL ITEMS--
select Count(*) from zbox.item  where 
userid in (Select userid from zbox.users where universityid=@userid)

---NEW Q&A--
		select countQ+countA from (
		SELECT  (
        select Count(*) from zbox.Question  where creationtime>getdate()-7
		and userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countQ,
        (
        select Count(*) from zbox.Answer  where creationtime>getdate()-7
		and userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countA
		) t

---ALL Q&A--
		select countQ+countA from (
		SELECT  (
        select Count(*) from zbox.Question  where 
		userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countQ,
        (
        select Count(*) from zbox.Answer  where 
		userid in (Select userid from zbox.users where universityid=@userid)
        ) AS countA
		) t
		
--National Cloudents Top 10--
select top 10 u.UniversityName as Name, (select count(*) from zbox.users where universityid = u.id) as students
from zbox.University u
where country = 'NL'
order by Students desc  ";
    }
}
