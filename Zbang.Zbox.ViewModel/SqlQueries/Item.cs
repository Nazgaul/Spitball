using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Item
    {
        public const string Navigation = @"with cte as ( select 
itemid,
LAG(Url) OVER (ORDER BY itemid) Prev,
LEAD(Url) OVER (ORDER BY itemid) Next
from zbox.item 
where boxid = @BoxId
and IsDeleted = 0)
select Prev,Next from  cte where itemid = @ItemId
";
    }
}
