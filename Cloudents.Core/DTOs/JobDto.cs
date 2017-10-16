using System;

namespace Cloudents.Core.DTOs
{
    public class JobDto
    {
        public string Title { get; set; }
        public string Responsibilities { get; set; }
        public DateTime? DateTime { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CompensationType { get; set; }
        public string Url { get; set; }
        public string Company { get; set; }
    }
}
