using System.ComponentModel.DataAnnotations;

namespace Cloudents.Ico.Models
{
    public class Subscribe
    {
        [Required]
        public string Email { get; set; }
    }
}