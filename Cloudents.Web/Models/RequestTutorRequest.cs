using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RequestTutorRequest
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public long? TutorId { get; set; }


        //[Captcha]
        //public string Captcha { get; set; }

    }
}