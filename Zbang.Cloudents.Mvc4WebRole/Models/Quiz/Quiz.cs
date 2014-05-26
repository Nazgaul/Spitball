using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class Quiz
    {
        [Required]
        public long BoxId { get; set; }
        public string Name { get; set; }
    }
}