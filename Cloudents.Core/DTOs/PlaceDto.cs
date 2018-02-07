using System.Runtime.Serialization;
using Cloudents.Core.Models;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class PlaceDto 
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Rating { get; set; }
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
        //public string Url {
        //    get => Name;
        //    set => Name = value;
        //}

        //public string Source => "Places" + (Hooked ? " Hooked" : string.Empty);
    }
}
