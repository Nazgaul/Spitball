using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Entities;

namespace Cloudents.Web.Models
{
    public class CreateAnswerRequest
    {
        [Required(ErrorMessage = "Required")]
        public long QuestionId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Range(Answer.MinLength, Answer.MaxLength, ErrorMessage = "MinLength")]
        public string Text { get; set; }

        [MaxLength(4,ErrorMessage = "MaxLength")]
        public string[] Files { get; set; }
       

    }
}