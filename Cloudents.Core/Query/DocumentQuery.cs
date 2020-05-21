using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Query
{
    public class DocumentQuery 
    {
        public DocumentQuery(UserProfile userProfile, string term, string course, int page, int pageSize, DocumentType? filter)
        {
            Filter = filter;
            PageSize = pageSize;
            Term = term;
            Course = course;
            Page = page;
            UserProfile = userProfile;
        }

        public DocumentType? Filter { get; }
        public int PageSize { get; }

        public string Term { get; }
        public string Course { get; }

        public int Page { get; set; }

        public UserProfile UserProfile { get; }
    }

   
}