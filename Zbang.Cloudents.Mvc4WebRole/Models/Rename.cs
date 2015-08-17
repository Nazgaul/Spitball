using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class Rename
    {
        [Required]
        public string NewName { get; set; }
        [Required]
        public long Id { get; set; }
    }
}