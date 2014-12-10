using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class FlagBadItem
    {
        [Required]
        public BadItem BadItem { get; set; }

        public string Other { get; set; }

        [Required]
        public long ItemId { get; set; }
    }
}