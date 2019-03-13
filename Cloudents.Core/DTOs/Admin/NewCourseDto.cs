using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class NewCourseDto
    {
        public string NewCourse { get; set; }
        public string OldCourse { get; set; }
    }

    public class NewUniversitiesDto
    {
        public Guid NewId{ get; set; }
        public string NewUniversity { get; set; }
        public Guid OldId { get; set; }
        public string OldUniversity { get; set; }

    }
}
