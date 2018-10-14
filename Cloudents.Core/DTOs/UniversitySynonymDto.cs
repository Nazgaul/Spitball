using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    //public class UniversitySynonymDto
    //{
    //    public IEnumerable<string> Name { get; set; }
    //}


    public class UniversityCoursesSynonymDto
    {
        public IEnumerable<string> University { get; set; }
        public IEnumerable<string> Courses { get; set; }
    }
}