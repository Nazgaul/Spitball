using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class NewAnnotation
    {
        [Required(AllowEmptyStrings=false)]
        public string Comment { get; set; }

        [Required]
        public int X { get; set; }
        [Required]
        public int Y { get; set; }
        [Required]
        public int Width { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public long ItemId { get; set; }
        [Required]
        public int ImageId { get; set; }
    }
}