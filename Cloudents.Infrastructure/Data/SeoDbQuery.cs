namespace Cloudents.Infrastructure.Data
{
    public sealed class SeoDbQuery
    {
        public string Query { get; }
        private SeoDbQuery(string str)
        {
            Query = str;
        }

        private const string DocumentSeoQuery = @"WITH boxSeo as (
 select BoxId, BoxName,u.UniversityName from zbox.box b JOIN zbox.University u on u.id = b.University
and Discriminator = 2
and b.IsDeleted = 0
)
select b.*,i.ItemId as id,i.Name
from zbox.item i join boxSeo b on i.BoxId = b.BoxId
where i.IsDeleted = 0
and i.content is not null
and i.Discriminator = 'FILE'
order by boxId
offset (@pageNumber)*@rowsPerPage ROWS
FETCH NEXT @rowsPerPage ROWS ONLY";

        private const string FlashcardSeoQuery = @"with boxSeo as (
 select BoxId, BoxName,u.UniversityName from zbox.box b join zbox.University u on u.id = b.University and needCode = 0
and Discriminator = 2
and b.IsDeleted = 0
)
select b.*,q.Id,q.Name
from zbox.Flashcard q join boxSeo b on q.BoxId = b.BoxId
where q.IsDeleted = 0
and q.publish = 1
order by boxId
offset (@pageNumber)*@rowsPerPage ROWS
FETCH NEXT @rowsPerPage ROWS ONLY";

        public static readonly SeoDbQuery Document = new SeoDbQuery(DocumentSeoQuery);
        public static readonly SeoDbQuery Flashcard = new SeoDbQuery(FlashcardSeoQuery);
    }
}