﻿namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class Jared
    {
        public const string ItemInfo = @"select
top 10 i.itemid itemId,
i.name itemName,
b.boxname boxName,
d.name department,
i.blobname blob,
b.boxid boxId,
bt.itemtabname docType,
bt.itemtabid typeId,
u.universityname university 
from " + ItemWhere+";";
        private const string ItemWhere = @"zbox.item i 
left join zbox.itemtaB bt ON i.itemtabid = bt.itemtabid
join zbox.box b ON i.boxid=b.boxid 
join zbox.library d ON b.libraryid=d.libraryId 
join zbox.university u ON b.university=u.id 
where b.discriminator = 2 and i.isdeleted=0 and b.university=173408 
and (@name is null or (i.name like +'%' + @name + '%'))
and (@university is null or (u.universityname like +'%' + @university + '%'))
and (@IsSearchType=0 or (i.itemtabid is null)) 
and (@IsReviewed=0 or (i.isreviewed=1)) 
and (@IsReviewed=1 or (i.isreviewed is null or i.isreviewed=0)) 
and (@box is null or (b.boxname like +'%' + @box + '%')) 
and (@department is null or (d.name like +'%' + @department + '%')) 
and (@boxid is null or (b.boxid=@boxid))";

        public const string ItemTabs = @"select 
tab.ItemTabId as id,
item.itemid as itemId,
itemtabname as name 
from zbox.ItemTab tab 
join zbox.item item ON tab.boxid=item.boxid 
where item.itemid in (select top 10 i.itemid from " + ItemWhere + ");";

        public const string ItemTags = @"select item.itemid itemId,
                                        t.name tag
from zbox.item item join zbox.itemtag it on item.itemid=it.itemid join zbox.tag t on it.tagid=t.id
where it.itemid in (select top 10 i.itemid from " + ItemWhere + ");";
    }
}
