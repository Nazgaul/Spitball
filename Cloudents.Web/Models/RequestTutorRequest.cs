using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RequestTutorRequest
    {
        public string? Text { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Course { get; set; }

        public long? TutorId { get; set; }

        public bool MoreTutors { get; set; }

        [Captcha]
        public string Captcha { get; set; }

    }
}