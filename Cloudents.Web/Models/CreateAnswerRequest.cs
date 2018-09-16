using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Filters;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class CreateAnswerRequest
    {
        [Required]
        public long QuestionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(15)]
        public string Text { get; set; }

        [ArrayMaxSize(4)]
        public IEnumerable<string> Files { get; set; }
       

    }
}