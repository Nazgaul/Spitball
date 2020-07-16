using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.DTOs.Admin
{
    public class PendingTutorsDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Bio { get; set; }
       
        public string Email { get; set; }

        public string? Image { get; set; }

        public string Courses => string.Join(", ", Courses2.Take(10));


        public IEnumerable<string> Courses2 { get; set; }


        public bool ShouldSerializeCourses2() => false;

        public DateTime? Created { get; set; }
    }
}
