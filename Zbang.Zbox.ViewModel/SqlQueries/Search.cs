using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Search
    {
        public const string OwnedSubscribedBoxes = @"select  b.BoxPicture as image,
 b.BoxName as name,
 b.ProfessorName as proffessor,
 b.CourseCode as courseCode,
 b.BoxId as id ,
 case b.Discriminator when 2 then (select universityname from zbox.Users u where b.OwnerId = u.UserId)
								                    else null
								                    end as universityname
from zbox.box b 
join zbox.UserBoxRel ub on ub.BoxId = b.BoxId and ub.UserId = @userId
where b.IsDeleted = 0
 and (b.BoxName like '%' + @query + '%'
	or b.CourseCode like '%' + @query + '%'
	or b.ProfessorName like '%' + @query + '%')
order by len(b.BoxName) - len(REPLACE(b.BoxName,@query,'')) / len(@query) asc, len(b.boxName)
offset @offsetV rows
fetch next @pageSize rows only; ";

        public const string UniversityBoxes = @"select b.BoxPicture as image,
 b.BoxName as name,
 b.ProfessorName as proffessor,
 b.CourseCode as courseCode,
 b.BoxId as id ,
 (select universityname from zbox.Users u where b.OwnerId = u.UserId) as universityname
from zbox.box b
where b.IsDeleted = 0
and b.OwnerId = @universityId
and b.Discriminator = 2
and (b.BoxName like '%' + @query + '%'
	or b.CourseCode like '%' + @query + '%'
	or b.ProfessorName like '%' + @query + '%')
order by len(b.BoxName) - len(REPLACE(b.BoxName,@query,'')) / len(@query) asc, len(b.boxName)
offset @offsetV rows
fetch next @pageSize rows only;";

        public const string Users = @"select  u.UserImageLarge as image,u.UserName as name, u.UserId as id
from zbox.users u
where u.UniversityId2 = @universityId
and u.username like '%' +@query + '%'
order by len(u.username) - len(REPLACE(u.username,@query,'')) / len(@query) asc
offset @offsetV rows
fetch next @pageSize rows only;";


        public const string Items = @"select 
i.ThumbnailBlobName as image,
i.Name as name,
i.ItemId as id,
i.Discriminator as type,
i.Content as content,
i.Rate as rate,
i.NumberOfViews as views,
b.BoxName as boxname,
b.BoxId as boxid ,u2.UniversityName as universityname
from zbox.item i
join zbox.box b on i.BoxId = b.BoxId and b.IsDeleted = 0
join zbox.users u2 on u2.UserId = b.OwnerId
where i.IsDeleted = 0
and b.OwnerId = @universityId
and b.Discriminator = 2
and i.Name like '%' +@query + '%'
order by len(i.Name) - len(REPLACE(i.name,@query,'')) / len(@query) asc
offset @offsetV rows
fetch next @pageSize rows only;";

        public const string ItemFromOtherUniversities = @"
select i.ThumbnailBlobName as image,
i.Name as name,
i.ItemId as id,
i.Discriminator as type,
i.Content as content,
i.Rate as rate,
i.NumberOfViews as views,
b.BoxName as boxname,
b.BoxId as boxid ,u2.UniversityName as universityname
from zbox.item i
join zbox.box b on i.BoxId = b.BoxId and b.IsDeleted = 0
join zbox.users u2 on u2.UserId = b.OwnerId
where i.IsDeleted = 0
and b.OwnerId in (
select userid from 
zbox.users u 
where u.NeedCode = 0 and u.UserType = 1 
and u.country = (select country from zbox.users where userid = @universityId)
)
and b.Discriminator = 2
and i.Name like '%' +@query + '%'
order by len(i.Name) - len(REPLACE(i.name,@query,'')) / len(@query) asc
offset @offsetV rows
fetch next @pageSize rows only;";
    }
}
