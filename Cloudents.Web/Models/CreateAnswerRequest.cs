using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateAnswerRequest
    {
        [Required]
        public long QuestionId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }
        public IEnumerable<string> Files { get; set; }
    }
}