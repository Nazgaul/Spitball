using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class DocumentDto
    {
        private DateTime _date;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime Date
        {
            get => DateTime.SpecifyKind(_date, DateTimeKind.Utc);
            set => _date = value;
        }

        [DataMember]
        public string Owner { get; set; }

        [DataMember]
        public string Blob { get; set; }

        public string Type { get; set; }
    }
}
