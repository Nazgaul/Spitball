
using System.ComponentModel.DataAnnotations;


namespace Zbang.Zbox.WebApi.Models
{
    public class LogInModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}