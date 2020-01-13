using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class UserDocumentsQuery : IQuery<ListWithCountDto<DocumentFeedDto>>
    {
        public UserDocumentsQuery(long id, int page, int pageSize)
        {
            Id = id;
            Page = page;
            PageSize = pageSize;
        }

        public long Id { get; }
        public int Page { get; }
        public int PageSize { get; set; }
    }
}
