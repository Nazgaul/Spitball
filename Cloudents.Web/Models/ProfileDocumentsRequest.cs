using Cloudents.Core.Enum;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ProfileDocumentsRequest
    {
        //public ProfileDocumentsRequest(long id, int page, DocumentType? documentType, string course,
        //    int pageSize = 20)
        //{
        //    Id = id;
        //    Page = page;
        //    DocumentType = documentType;
        //    Course = course;
        //    PageSize = pageSize;
        //}
        [Required]
        [FromRoute]
        public long Id { get; set; }
        [Required]
        public int Page { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? Course { get; set; }
        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;
    }
}
