using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class CreateNoteRequest
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        [StringLength(4000)]
        public string Text { get; set; }
    }
}
