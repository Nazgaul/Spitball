using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class MigrateCourseRequest
    {
        public string CourseToRemove { get; set; }
        public string CourseToKeep { get; set; }
    }
}
