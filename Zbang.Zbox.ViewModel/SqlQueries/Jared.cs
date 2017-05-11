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
where b.discriminator = 2 and i.isdeleted=0 and b.university in(173408,166100,171885,172566) 
and (@name is null or (i.name like +'%' + @name + '%'))
and (@university is null or (u.universityname like +'%' + @university + '%'))
and (@IsSearchType=0 or (i.doctype is null)) 
and (@IsSearchType=1 or (i.doctype is not null)) 
and (@IsReviewed=0 or (i.isreviewed=1)) 
and (@IsReviewed=1 or (i.isreviewed is null or i.isreviewed=0)) 
and (@box is null or (b.boxname like +'%' + @box + '%')) 
and (@department is null or (d.name like +'%' + @department + '%')) 
and (@boxid is null or (b.boxid=@boxid))";

        public const string ItemTags = @"select item.itemid itemId,
                                        t.name tag
from zbox.item item join zbox.itemtag it on item.itemid=it.itemid join zbox.tag t on it.tagid=t.id
where it.itemid in (select top 10 i.itemid from " + ItemWhere + ");";

        public const string AutoUni = @"SELECT TOP 5 u.universityname 
        from [Zbox].[University] u where u.universityname like +'%'+@term+'%';";

        public const string AutoDepartment = @"SELECT TOP 5 d.name from zbox.library d where d.name like +'%'+@term+'%';";
        public const string AutoTag = @"SELECT TOP 5 t.name from zbox.tag t where t.name like +'%'+@term+'%';";


        public const string DocumentFavorites = @"select 
itemid as id,
i.Name as name,
BlobName as source,
content as meta,
i.LikeCount as likes,
i.NumberOfViews as views,
i.CreationTime as date
 from zbox.item i
where itemid in @documentIds;";

        public const string QuizFavorites = @"select 
id as id,
q.Name as name,
q.LikeCount as likes,
  q.NumberOfViews as views,
   q.CreationTime as date,
(select top 1 text from zbox.QuizQuestion where quizid = q.id order by id)  as source,
(select count(*) from zbox.quizquestion where quizid = q.id) as numOfQuestion
 from zbox.quiz q
where id in @quizIds;";

        //Merge with document db
        public const string FlashcardFavorite = @"select 
id as id,
f.LikeCount as likes,
  f.NumberOfViews as views
 from zbox.flashcard f
where id in @flashcardIds;";

        public const string CommentFavorite = @"select 
q.questionid as id,
u.userimagelarge as image,
u.username as name,
q.text,
q.creationtime as date,
q.replycount as replies,
(select count(*) from zbox.item  i where i.questionid = q.questionid and i.isdeleted = 0) as items
 from zbox.question q join zbox.users u on q.userid = u.userid
where questionid in @commentIds;";
    }
}
