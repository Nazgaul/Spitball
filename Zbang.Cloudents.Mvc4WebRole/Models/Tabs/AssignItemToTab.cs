using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Tabs
{
    public class AssignItemToTab
    {
        [Required]
        public long BoxId { get; set; }
        [Required]
        public Guid TabId { get; set; }
        
        // the list can be empty
        public long[] ItemId { get; set; }

        public bool nDelete { get; set; }
    }
}