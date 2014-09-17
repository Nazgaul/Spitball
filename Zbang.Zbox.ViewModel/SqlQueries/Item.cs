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
LAG(Url) OVER (ORDER BY itemid) Previous,
LEAD(Url) OVER (ORDER BY itemid) Next
from zbox.item 
where boxid = @BoxId
and IsDeleted = 0)
select Previous,Next from  cte where itemid = @ItemId;
";

        public const string ItemDetail = @" select 
 i.itemid as id, 
 i.Name as name,
 i.UpdateTime as updateTime,
    u.UserName as owner, 
    u.userid as ownerId,
	i.NumberOfViews as numberOfViews,
	i.numberofdownloads as numberOfDownloads,
    i.BlobName as blob,
    i.rate as Rate,
    b.BoxName as BoxName,
    b.Url as BoxUrl
    from zbox.Item i
    join zbox.Users u on u.UserId = i.UserId
    join zbox.box b on b.BoxId=i.BoxId
    where i.ItemId = @ItemId
    and i.IsDeleted = 0 
	and i.boxid = @BoxId;";


        public const string ItemComments = @"SELECT [ItemCommentId] as Id
                          ,[Comment]
                          ,ic.CreationTime as CreationDate
	                      ,u.UserName
                          ,u.userid as UserId
  FROM [Zbox].[ItemComment] ic join zbox.Users u on ic.UserId = u.UserId
                      where itemid = @ItemId
                      order by ic.CreationTime desc;";
    }
}
