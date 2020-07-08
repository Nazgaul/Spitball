using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ProfileDocumentsRequest
    {
        [Required]
        [FromRoute]
        public long Id { get; set; }
    }
}
