using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class SessionDto
    {
        public DateTime Created { get; set; }
        public string Tutor { get; set; }
        public string Student { get; set; }
        public int Duration { get; set; }
    }
}
