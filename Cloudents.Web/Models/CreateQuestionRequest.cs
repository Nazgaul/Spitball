using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Enum;
using Cloudents.Web.Filters;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class CreateQuestionRequest
    {
        [Required]
        public int SubjectId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(15)]
        public string Text { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [ArrayMaxSize(4)]
        public IEnumerable<string> Files { get; set; }

        public QuestionColor? Color { get; set; }


      
    }
}
