using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class ChangeBoxTabName
    {
        [Required]
        public Guid TabId { get; set; }

        [Required(AllowEmptyStrings=false)]
        public string NewName { get; set; }
    }
}