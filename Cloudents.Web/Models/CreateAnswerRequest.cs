using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateAnswerRequest
    {
        [Required(ErrorMessage = "Required")]
        public long QuestionId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(15,ErrorMessage = "MinLength")]
        public string Text { get; set; }

        [MaxLength(4,ErrorMessage = "MaxLength")]
        public string[] Files { get; set; }
       

    }
}