using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class CreateLibraryItem
    {
        [Required]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
    }
}