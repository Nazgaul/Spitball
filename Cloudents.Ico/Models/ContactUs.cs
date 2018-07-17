using System.ComponentModel.DataAnnotations;

namespace Cloudents.Ico.Models
{
    public class ContactUs
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Text { get; set; }
    }
}