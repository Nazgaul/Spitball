using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Range(0.1, 100)]
        public decimal Price { get; set; }

        //[MaxLength(4)]
        public IEnumerable<string> Files { get; set; }
    }
}
