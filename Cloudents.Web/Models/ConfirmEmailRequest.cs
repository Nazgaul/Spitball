using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class ConfirmEmailRequest
    {

        [Required]
        public long? Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }
    }
}