using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Tabs
{
    public class ChangeBoxItemTabName
    {
        [Required]
        public Guid TabId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string NewName { get; set; }

        [Required]
        public long BoxUid { get; set; }
    }
}