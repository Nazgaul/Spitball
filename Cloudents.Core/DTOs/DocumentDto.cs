using System;

namespace Cloudents.Core.DTOs
{
    public class DocumentDto
    {
        private DateTime _date;
        public string Name { get; set; }

        public DateTime Date
        {
            get => DateTime.SpecifyKind(_date, DateTimeKind.Utc);
            set => _date = value;
        }

        public string Owner { get; set; }

        public string Blob { get; set; }

        public string Type { get; set; }
    }
}
