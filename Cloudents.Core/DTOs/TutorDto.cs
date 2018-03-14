using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Cloudents.Core.Models;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Dto class return as Json")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class return as Json")]
    public class TutorDto : IUrlRedirect , IShuffleable
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public double? Fee { get; set; }

        public GeoPoint Location { get; set; } //mobile application use that
        public string Description { get; set; }

        public bool? Online { get; set; }

        public string Source => PrioritySource.ToString();

        [IgnoreDataMember]
        public PrioritySource PrioritySource { get; set; }

        [IgnoreDataMember]
        public int Order { get; set; }
    }
}
