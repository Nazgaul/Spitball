
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
select Prev as Previous,Next from  cte where itemid = @ItemId
";

        public const string ItemDetail = @"select 
 i.Name as name,
 i.creationTime as date,
    u.UserName as owner, 
    u.userid as ownerId,
    u.url as ownerUrl,
	i.NumberOfViews as numberOfViews,
	i.numberofdownloads as numberOfDownloads,
    i.BlobName as blob,
    i.Discriminator as type,
    i.likeCount as Likes
    --b.Url as BoxUrl
    from zbox.Item i
    join zbox.Users u on u.UserId = i.UserId
    where i.ItemId = @ItemId
    and i.IsDeleted = 0 ;";

        public const string ItemDetailApi = @" select 
 i.itemid as Id,
 i.Size,
 i.NumberOfViews as numberOfViews,
 i.Url,
 b.boxname,
 i.creationTime,
 i.Discriminator as Type,
 i.BlobName as Source,
 i.Name as name,
    u.UserName as owner, 
    u.userid as ownerId,
	i.numberofdownloads as numberOfDownloads
    from zbox.Item i
    join zbox.Users u on u.UserId = i.UserId
    join zbox.box b on b.BoxId=i.BoxId and b.isdeleted = 0
    where i.ItemId = @ItemId
    and i.IsDeleted = 0;";

        public const string UserItemRate =
            @"select ir.Rate from zbox.ItemRate ir where ir.ItemId = @ItemId and ir.OwnerId = @UserId;";


        public const string ItemComments = @"SELECT [ItemCommentId] as Id
                          ,[Comment]
                          ,ic.CreationTime as CreationDate
	                      ,u.UserName
,u.UserImageLarge as UserImage
,u.url as UserUrl
                          ,u.userid as UserId
  FROM [Zbox].[ItemComment] ic join zbox.Users u on ic.UserId = u.UserId
                      where itemid = @ItemId
                      order by ic.CreationTime desc;";

        public const string ItemCommentReply = @"SELECT [ItemReplyId] as Id
,[Comment]
,cr.CreationTime as CreationDate
,u.UserName as UserName
    ,u.UserImageLarge as UserImage
    ,u.url as UserUrl
      ,u.UserId
      ,[ParentId]
      
  FROM [Zbox].[ItemCommentReply] cr join zbox.Users u on cr.UserId = u.userid
  where itemid = @ItemId
  order by cr.CreationTime desc;";
    }
}
