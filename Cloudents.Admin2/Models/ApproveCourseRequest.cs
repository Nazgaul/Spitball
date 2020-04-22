using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class ApproveCourseRequest
    {
        [Required]
        public string Course { get; set; }
        public string Subject { get; set; }
       
    }

    public class ApproveUniversityRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
