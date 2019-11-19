using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class DropBoxRequest
    {
        public string Name { get; set; }
        [Required] public Uri Link { get; set; }
        public long Size { get; set; }
    }
}
