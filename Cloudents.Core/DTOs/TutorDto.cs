using System;
using System.Runtime.Serialization;
using Cloudents.Core.Models;
using Cloudents.Core.Request;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class TutorDto
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Image { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }

        [DataMember]
        public double? Fee { get; set; }

        [DataMember]
        public bool? Online { get; set; }

        [DataMember]
        public GeoPoint Location { get; set; }

        public int TermFound { get; set; }
    }
}
