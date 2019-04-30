using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class CreateQuestionRequest
    {
        public QuestionSubject? SubjectId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [StringLength(8000, MinimumLength = 15, ErrorMessage = "MinLength")]
        public string Text { get; set; }

        [MaxLength(4, ErrorMessage = "MaxLength")]
        public string[] Files { get; set; }

        [Required]
        public string Course { get; set; }
    }
}
    
