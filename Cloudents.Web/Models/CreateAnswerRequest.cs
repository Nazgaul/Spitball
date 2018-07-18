using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateAnswerRequest
    {
        [Required]
        public long QuestionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(15)]
        public string Text { get; set; }

        //[MaxLength(4)]
        public IEnumerable<string> Files { get; set; }
    }
}