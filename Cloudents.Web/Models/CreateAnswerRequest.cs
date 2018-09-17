using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Filters;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class CreateAnswerRequest
    {
        [Required(ErrorMessage = "Required")]
        public long QuestionId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(15)]
        public string Text { get; set; }

        [ArrayMaxSize(4)]
        public IEnumerable<string> Files { get; set; }
       

    }
}