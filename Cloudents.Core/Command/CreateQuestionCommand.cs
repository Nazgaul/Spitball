using Cloudents.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Core.Command
{
    public class CreateQuestionCommand : ICommand
    {
        [Required]
        public long SubjectId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }

        [Required]
        [Range(0.1, 1000)]
        public decimal Price { get; private set; }

        public long UserId { get; set; }
    }
}