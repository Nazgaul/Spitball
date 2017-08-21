using System;
using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class AddFromDropBox
    {
        [Required(ErrorMessageResourceType = typeof(AddLinkResources), ErrorMessageResourceName = "LinkRequired")]
        public string Url { get; set; }
        [Required]
        public long BoxId { get; set; }

        public bool Question { get; set; }

        public string Name { get; set; }

        public Guid? TabId { get; set; }
    }
}