using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CodeRequest
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "StringLength")]
        public string Number { get; set; }

    }
}