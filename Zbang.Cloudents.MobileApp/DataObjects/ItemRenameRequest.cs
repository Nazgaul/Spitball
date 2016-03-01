using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class ItemRenameRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string  Name{ get; set; }
    }
}