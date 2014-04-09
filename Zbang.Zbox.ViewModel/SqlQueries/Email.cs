using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Email
    {
        public const string partners = @"
--NEW USERS--
Select Count(*) from zbox.users where creationtime>getdate()-7
and universityid2=@userid

--ALL USERS--
Select Count(*) from zbox.users where universityid2=@userid

--NEW COURSES--
select Count(*) from zbox.box  where creationtime>getdate()-7 and ownerid=@userid

--ALL COURSES--
select Count(*) from zbox.box  where ownerid=@userid

--NEW ITEMS--
select Count(*) from zbox.item  where creationtime>getdate()-7
and userid in (Select userid from zbox.users where universityid2=@userid)





--ALL ITEMS--
select Count(*) from zbox.item  where 
userid in (Select userid from zbox.users where universityid2=@userid)

---NEW Q&A--
		select countQ+countA from (
		SELECT  (
        select Count(*) from zbox.Question  where creationtime>getdate()-7
		and userid in (Select userid from zbox.users where universityid2=@userid)
        ) AS countQ,
        (
        select Count(*) from zbox.Answer  where creationtime>getdate()-7
		and userid in (Select userid from zbox.users where universityid2=@userid)
        ) AS countA
		) t

---ALL Q&A--
		select countQ+countA from (
		SELECT  (
        select Count(*) from zbox.Question  where 
		userid in (Select userid from zbox.users where universityid2=@userid)
        ) AS countQ,
        (
        select Count(*) from zbox.Answer  where 
		userid in (Select userid from zbox.users where universityid2=@userid)
        ) AS countA
		) t
		
--National Cloudents Top 10--
select top 10 u.username as Name, (select count(*) from zbox.users where universityid2 = u.userid) as students
from zbox.users u where usertype = 1
and country = 'NL'
order by Students desc  ";
    }
}
