using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class MailRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Body { get; set; }
    }
}