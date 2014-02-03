using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class AssignBoxToTab
    {
        [Required]
        public string BoxId { get; set; }
        [Required]
        public Guid TabId { get; set; }
    }
}