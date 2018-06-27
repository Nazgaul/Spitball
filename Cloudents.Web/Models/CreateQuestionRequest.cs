using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateQuestionRequest
    {
        [Required]
        public int SubjectId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }

        [Required]
        [Range(0.1, 1000)]
        public decimal Price { get; set; }

        [MaxLength(4)]
        public IEnumerable<string> Files { get; set; }
    }
}
