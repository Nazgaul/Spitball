
using System.ComponentModel.DataAnnotations;


namespace Zbang.Zbox.WebApi.Models
{
    public class LogInVerify
    {
        [Required]
        public string Token { get; set; }
    }
}