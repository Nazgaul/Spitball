using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Tabs
{
    public class AssignItemToTab
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        [Required]
        public long BoxId { get; set; }

        public Guid? TabId { get; set; }
        
        [Range(1,long.MaxValue)]
        public long ItemId { get; set; }

 // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}