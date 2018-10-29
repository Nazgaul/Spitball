using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class DropBoxRequest
    {
        public string Name { get; set; }
        [Required]public Uri Link { get; set; }
        public long Size { get; set; }
    }
}
