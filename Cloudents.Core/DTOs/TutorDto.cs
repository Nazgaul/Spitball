﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Cloudents.Core.Models;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Dto class return as Json")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class return as Json")]
    public class TutorDto : IUrlRedirect , IShuffleable
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
        public GeoPoint Location { get; set; } //mobile application use that
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool? Online { get; set; }

        [DataMember]
        public string Source => PrioritySource.ToString();

        public PrioritySource PrioritySource { get; set; }

        public int Order { get; set; }
    }
}
