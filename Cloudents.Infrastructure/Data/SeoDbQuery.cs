//namespace Cloudents.Infrastructure.Data
//{
//    public sealed class SeoDbQuery
//    {
//        public string Query { get; }

//        private SeoDbQuery(string str)
//        {
//            Query = str;
//        }

//        private const string FlashcardSeoQuery = @"with boxSeo as (
// select BoxId, BoxName,u.UniversityName from zbox.box b join zbox.University u on u.id = b.University and needCode = 0
//and Discriminator = 2
//and b.IsDeleted = 0
//)
//select b.*,q.Id,q.Name
//from zbox.Flashcard q join boxSeo b on q.BoxId = b.BoxId
//where q.IsDeleted = 0
//and q.publish = 1
//order by boxId
//offset (@pageNumber)*@rowsPerPage ROWS
//FETCH NEXT @rowsPerPage ROWS ONLY";

//        public static readonly SeoDbQuery Flashcard = new SeoDbQuery(FlashcardSeoQuery);
//    }
//}