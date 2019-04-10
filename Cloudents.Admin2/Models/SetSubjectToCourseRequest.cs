using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class SetSubjectToCourseRequest
    {
        [Required]
        public string CourseName { get; set; }

        [Required]
        public string Subject { get; set; }
    }
}
