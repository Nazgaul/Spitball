using System.ComponentModel.DataAnnotations;
using Cloudents.Common;
using Cloudents.Core.Enum;
using Cloudents.Domain.Enums;

namespace Cloudents.Web.Models
{
    public class CreateQuestionRequest
    {
        [Required(ErrorMessage = "Required")]
        public QuestionSubject? SubjectId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(15,ErrorMessage = "MinLength")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(1, 100,ErrorMessage = "Range")]
        public decimal Price { get; set; }

        [MaxLength(4, ErrorMessage = "MaxLength")]
        public string[] Files { get; set; }

        public QuestionColor? Color { get; set; }


      
    }

    public class CreateQuestionResponse
    {
        public CreateQuestionResponse(string toasterText)
        {
            ToasterText = toasterText;
        }

        public string ToasterText { get; set; }
    }
}
