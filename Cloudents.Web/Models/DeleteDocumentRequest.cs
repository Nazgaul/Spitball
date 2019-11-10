using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class DeleteDocumentRequest
    {
        [FromRoute]
        [Range(1, long.MaxValue, ErrorMessage = "Range")]
        public long Id { get; set; }
    }
}
