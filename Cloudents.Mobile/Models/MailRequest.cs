using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class MailRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Body { get; set; }
    }
}