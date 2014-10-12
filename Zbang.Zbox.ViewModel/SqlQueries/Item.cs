
namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Item
    {
        public const string Navigation = @"WITH CTE AS (
SELECT
rownum = ROW_NUMBER() OVER (ORDER BY p.itemid),
p.url, p.itemid
FROM zbox.item p
where p.boxid = @BoxId
and IsDeleted = 0
)
SELECT
prev.url Previous,
nex.url Next
FROM CTE
LEFT JOIN CTE prev ON prev.rownum = CTE.rownum - 1
LEFT JOIN CTE nex ON nex.rownum = CTE.rownum + 1
where cte.itemid = @itemid;
";

        public const string ItemDetail = @" select 
 i.Name as name,
 i.UpdateTime as updateTime,
    u.UserName as owner, 
    u.userid as ownerId,
	i.NumberOfViews as numberOfViews,
	i.numberofdownloads as numberOfDownloads,
    i.BlobName as blob,
    b.BoxName as BoxName,
    b.Url as BoxUrl
    from zbox.Item i
    join zbox.Users u on u.UserId = i.UserId
    join zbox.box b on b.BoxId=i.BoxId and b.isdeleted = 0
    where i.ItemId = @ItemId
    and i.IsDeleted = 0 
	and i.boxid = @BoxId;";

        public const string UserItemRate =
            @"select ir.Rate from zbox.ItemRate ir where ir.ItemId = @ItemId and ir.OwnerId = @UserId;";


        public const string ItemComments = @"SELECT [ItemCommentId] as Id
                          ,[Comment]
                          ,ic.CreationTime as CreationDate
	                      ,u.UserName
                          ,u.userid as UserId
  FROM [Zbox].[ItemComment] ic join zbox.Users u on ic.UserId = u.UserId
                      where itemid = @ItemId
                      order by ic.CreationTime desc;";

        public const string ItemCommentReply = @"SELECT [ItemReplyId] as Id
,[Comment]
,cr.CreationTime as CreationDate
,u.UserName as UserName
    
      ,u.UserId
      ,[ParentId]
      
  FROM [Zbox].[ItemCommentReply] cr join zbox.Users u on cr.UserId = u.userid
  where itemid = @ItemId
  order by cr.CreationTime desc;";
    }
}
