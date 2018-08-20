using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Web.Binders;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
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
