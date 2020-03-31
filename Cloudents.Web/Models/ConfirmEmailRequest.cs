using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ConfirmEmailRequest
    {

        [Required(ErrorMessage = "Required")]
        public long? Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Code { get; set; }
      //  public string ReturnUrl { get; set; }

      //  public string Referral { get; set; }
    }
}