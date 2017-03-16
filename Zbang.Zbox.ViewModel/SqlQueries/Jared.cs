namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class Jared
    {
        public const string ItemInfo = @"select
top 10 i.itemid itemId,
i.name itemName,
b.boxname boxName,
d.name department,
i.blobname blob,
bt.itemtabname docType
from zbox.item i 
left join zbox.itemtaB bt ON i.itemtabid = bt.itemtabid
join zbox.box b ON i.boxid=b.boxid 
join zbox.library d ON b.libraryid=d.libraryId 
where b.discriminator = 2 and i.isdeleted=0
and (@name is null or (i.name like +'%' + @name + '%'))
and (@university is null or (b.university in (select u.id from zbox.university u where u.universityname like +'%' + @university + '%')))
and (@IsSearchType=0 or (i.itemtabid is null))
and (@box is null or (b.boxname like +'%' + @box + '%'))
and (@department is null or (d.name like +'%' + @department + '%'))
and (@boxid is null or (b.coursecode=@boxid));";
        private const string ItemWhere = @"zbox.item i 
left join zbox.itemtaB bt ON i.itemtabid = bt.itemtabid
join zbox.box b ON i.boxid=b.boxid 
join zbox.library d ON b.libraryid=d.libraryId 
where b.discriminator = 2 and i.isdeleted=0
and (@name is null or (i.name like +'%' + @name + '%'))
and (@university is null or (b.university in (select u.id from zbox.university u where u.universityname like +'%' + @university + '%')))
and (@IsSearchType=0 or (i.itemtabid is null))
and (@box is null or (b.boxname like +'%' + @box + '%'))
and (@department is null or (d.name like +'%' + @department + '%'))
and (@boxid is null or (b.coursecode=@boxid))";

        public const string ItemTags = @"select item.itemid itemId,
                                        t.name tag
from zbox.item item join zbox.itemtag it on item.itemid=it.itemid join zbox.tag t on it.tagid=t.id
where it.itemid in(select top 10 i.itemid from " + ItemWhere + ");";
    }
}
