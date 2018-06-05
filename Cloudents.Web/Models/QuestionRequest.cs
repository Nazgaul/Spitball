using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class QuestionRequest
    {
        [Required]
        public int SubjectId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }

        [Required]
        [Range(0.1, 1000)]
        public decimal Price { get; set; }

        public IEnumerable<string> Files { get; set; }
    }

    public class QuestionsRequest : IPaging
    {
        public string[] Term { get; set; }
        public string[] Source { get; set; }
        public int? Page { get; set; }
    }
}
