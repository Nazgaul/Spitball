using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;

namespace Cloudents.Query.Query
{
    public class UserDocumentsQuery : IQuery<ListWithCountDto<DocumentFeedDto>>
    {
        public UserDocumentsQuery(long id, int page, int pageSize, DocumentType? documentType, string course)
        {
            Id = id;
            Page = page;
            PageSize = pageSize;
            DocumentType = documentType;
            Course = course;
        }

        public long Id { get; }
        public int Page { get; }
        public int PageSize { get; }

        public DocumentType? DocumentType { get; }
        public string Course { get;}
    }
}
