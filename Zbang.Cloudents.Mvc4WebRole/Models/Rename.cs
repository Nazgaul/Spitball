using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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