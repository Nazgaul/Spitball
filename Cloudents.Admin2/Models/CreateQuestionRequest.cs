using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class CreateQuestionRequest
    {
        [Required]
        public int SubjectId { get; set; }
        [Required] public string Text { get; set; }

        [Required] public decimal Price { get; set; }
    }
}
