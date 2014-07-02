using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Tabs
{
    public class DeleteBoxItemTab
    {
        [Required]
        public Guid TabId { get; set; }


        [Required]
        public long BoxId { get; set; }
    }
}