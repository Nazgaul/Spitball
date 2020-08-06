using System;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class UserCoursesDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime? StartOn { get; set; }

        public int Lessons { get; set; }

        public int Documents { get; set; }

        public int Users { get; set; }

        public Money Price { get; set; }
        public bool IsPublish { get; set; }
        public string Image { get; set; }
        
        [NonSerialized]
        public int Version;
    }
   
}
