using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class EditUserProfileRequest
    {
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Name { get; set; }
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Description { get; set; }
    }
}
