using Cloudents.Core.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class ApproveCourseRequest
    {
        [Required]
        public string Course { get; set; }
        public string Subject { get; set; }
        [Required]
        public SchoolType SchoolType { get; set; }
    }

    public class ApproveUniversityRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
