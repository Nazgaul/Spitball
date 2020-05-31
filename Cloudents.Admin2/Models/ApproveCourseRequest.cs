using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class ApproveCourseRequest
    {
        [Required]
        public string Course { get; set; }
        [Required]
        public string Subject { get; set; }
       
    }


}
