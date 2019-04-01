using System;

namespace Cloudents.Admin2.Models
{
    public class ApproveCourseRequest
    {
        public string Course { get; set; }
    }

    public class ApproveUniversityRequest
    {
        public Guid Id { get; set; }
    }
}
