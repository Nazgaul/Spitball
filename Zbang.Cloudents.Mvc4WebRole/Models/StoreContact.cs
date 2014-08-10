using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class StoreContact
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        public string University { get; set; }
        [Required]
        public String Email { get; set; }

        [Required]
        public string Text { get; set; }
    }
}