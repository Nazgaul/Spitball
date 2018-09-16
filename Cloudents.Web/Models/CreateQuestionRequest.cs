using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Enum;
using Cloudents.Web.Filters;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class CreateQuestionRequest
    {
        [Required(ErrorMessage = "Required")]
        public int SubjectId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(15)]
        public string Text { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [ArrayMaxSize(4)]
        public IEnumerable<string> Files { get; set; }

        public QuestionColor? Color { get; set; }


      
    }
}
