using System;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    public class JobDto
    {
        private string _responsibilities;
        public string Title { get; set; }

        public string Responsibilities
        {
            get => _responsibilities.RemoveEndOfString(300);
            set => _responsibilities = value;
        }

        public DateTime? DateTime { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CompensationType { get; set; }
        public string Url { get; set; }
        public string Company { get; set; }
    }
}
