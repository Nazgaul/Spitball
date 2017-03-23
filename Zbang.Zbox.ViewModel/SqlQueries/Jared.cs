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
b.boxid boxId,
i.doctype docType,
u.universityname university 
from " + ItemWhere + ";";
        private const string ItemWhere = @"zbox.item i 
join zbox.box b ON i.boxid=b.boxid 
join zbox.library d ON b.libraryid=d.libraryId 
join zbox.university u ON b.university=u.id 
where b.discriminator = 2 and i.isdeleted=0 and b.university=173408 
and (@name is null or (i.name like +'%' + @name + '%'))
and (@university is null or (u.universityname like +'%' + @university + '%'))
and (@IsSearchType=0 or (i.doctype is null)) 
and (@IsReviewed=0 or (i.isreviewed=1)) 
and (@IsReviewed=1 or (i.isreviewed is null or i.isreviewed=0)) 
and (@box is null or (b.boxname like +'%' + @box + '%')) 
and (@department is null or (d.name like +'%' + @department + '%')) 
and (@boxid is null or (b.boxid=@boxid))";

        public const string ItemTags = @"select item.itemid itemId,
                                        t.name tag
from zbox.item item join zbox.itemtag it on item.itemid=it.itemid join zbox.tag t on it.tagid=t.id
where it.itemid in (select top 10 i.itemid from " + ItemWhere + ");";

        public const string autoUni = @"SELECT TOP 5 u.universityname 
        from [Zbox].[University] u where u.universityname like +'%'+@term+'%';";

        public const string autoDepartment = @"SELECT TOP 5 d.name from zbox.library d where d.name like +'%'+@term+'%';";
        public const string autoTag = @"SELECT TOP 5 t.name from zbox.tag t where t.name like +'%'+@term+'%';";
    }
}
