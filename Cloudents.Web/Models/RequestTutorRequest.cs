using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RequestTutorRequest
    {
        public string? Text { get; set; }

        public long? TutorId { get; set; }


        [Captcha]
        public string Captcha { get; set; }

    }
}