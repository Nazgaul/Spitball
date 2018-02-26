using System.Collections.Generic;
using System.Runtime.Serialization;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class PlaceDto 
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public float Rating { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public bool Open { get; set; }

        [DataMember]
        public GeoPoint Location { get; set; }

        [DataMember]
        public string Image { get; set; }
        [DataMember]
        public string PlaceId { get; set; }

        [DataMember]
        public bool Hooked { get; set; }
    }


    public class PlacesNearbyDto
    {
        [CanBeNull]
        public IEnumerable<PlaceDto> Data { get; set; }
        [CanBeNull]
        public string Token { get; set; }
    }
}
