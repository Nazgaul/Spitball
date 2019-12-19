using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class RenameDocumentRequest
    {
        [Required]
        public long DocumentId { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "StringLength", MinimumLength = Core.Entities.Course.MinLength)]
        public string Name { get; set; }
    }
}
