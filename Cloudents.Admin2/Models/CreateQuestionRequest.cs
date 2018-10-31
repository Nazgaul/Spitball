using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class CreateQuestionRequest
    {
        [Required, Range(1, 100)]
        public int SubjectId { get; set; }
        [Required] public string Text { get; set; }

        [Required] public decimal Price { get; set; }
    }

    public class ApproveRequest
    {
        public long Id { get; set; }
    }
}
