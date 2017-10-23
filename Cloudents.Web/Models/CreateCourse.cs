using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class CreateCourse
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
