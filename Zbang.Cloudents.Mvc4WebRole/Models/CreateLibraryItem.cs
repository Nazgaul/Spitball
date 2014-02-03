using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateLibraryItem
    {
        [Required]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
    }
}